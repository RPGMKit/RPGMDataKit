using System.Collections.Concurrent;
using System.Reflection;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Commands.ParameterProviders;
using RPGMDataKit.Data.Structures.Commands.Helpers;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Storages;
using RPGMDataKit.Data.Structures.Attributes;

namespace RPGMDataKit.Data.Structures.Internal;

/// <summary>
/// A command with named properties. It is a view over a raw command: properties read and write the raw command's
/// parameters directly, so there is nothing to unpack, pack or keep in sync.
/// </summary>
public abstract class NamedCommand : IHasOptionalProperties
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> ParameterProperties = new();
    private static readonly ConcurrentDictionary<Type, (PropertyInfo Property, EditorDefaultBaseAttribute[] Defaults)[]> EditorDefaults = new();
    private static readonly MethodInfo NewEntityMethod = typeof(TypeMap).GetMethod(nameof(TypeMap.New))!;

    private ICommand? _raw;
    private bool _copy;
    private int _lastIndex = -1;
    private IParameterProvider? _parameters;
    private StorageVersion _version = StorageVersion.VXAce;
    private TypeMap? _types;

    public abstract int Code { get; }

    public abstract string GetCommandName(StorageVersion version);

    /// <summary>The raw command this command reads and writes. Until it is put into a container it is a private one.</summary>
    public ICommand Raw => _raw ??= new DetachedCommand { Code = Code };

    public int? Indent
    {
        get => Raw.Indent;
        set => Raw.Indent = value;
    }

    public bool IsSecondary;

    public T Cast<T>() where T : NamedCommand => (T)this;

    /// <summary>The list the command is in. A command that is in a list cannot be put into another one, use <see cref="Clone"/>.</summary>
    internal object? Owner { get; set; }

    /// <summary>
    /// A new command of the same class with the same property values and its own objects (sounds, tones, routes...),
    /// not in any list. Put into a list of any storage it gets the raw command of that storage.
    /// </summary>
    public NamedCommand Clone()
    {
        var copy = CreateBlank();
        copy._copy = true;   // it has what the original has, not what the editor gives to a new command
        copy.Indent = Indent;
        copy.IsSecondary = IsSecondary;

        foreach (var property in ParameterProperties.GetOrAdd(GetType(), FindParameterProperties))
        {
            // a property that the maker of this command does not have is left as it is
            if (HasProperty(property.Name))
                property.SetValue(copy, Copies.Of(property.GetValue(this)));
        }

        return copy;
    }

    /// <summary>
    /// Reads every property, so the objects the command keeps in its parameters (a sound, a tone, a route...) are objects
    /// of their classes and not the JSON objects of the file.
    /// </summary>
    internal virtual void Materialize()
    {
        // the end of a route can have no parameters in the file ({"code":0}); in the dump it has an empty list
        if (Raw.Parameters == null && Raw.Code == 0)
            Raw.Parameters = [];

        foreach (var property in ParameterProperties.GetOrAdd(GetType(), FindParameterProperties))
        {
            if (HasProperty(property.Name))
                property.GetValue(this);
        }
    }

    /// <summary>An empty command of the same class; a class without a parameterless constructor overrides it.</summary>
    protected virtual NamedCommand CreateBlank() => (NamedCommand)Activator.CreateInstance(GetType())!;

    /// <summary>
    /// False if the maker of the list that the command is in does not have the property (<see cref="DoesNotExistInVersionAttribute"/> with
    /// the makers). A command that is not in a list yet has no maker and has the property; it is dropped when the command gets a maker that
    /// has not.
    /// </summary>
    /// <exception cref="ArgumentException">The class of the command has no such property.</exception>
    public bool HasProperty(string propertyName) =>
        OptionalProperties.Has(GetType(), propertyName, IsDetached ? null : _version);

    /// <summary>
    /// For a value (not a property) that is of one maker only, as a blend mode: throws <see cref="DoesNotExistInVersionException"/> when the
    /// command is in a list of another maker. A command that is not in a list yet has no maker; it is checked when the command gets one.
    /// </summary>
    protected void RequireVersion(StorageVersion version, string property)
    {
        if (!IsDetached && _version != version)
            throw new DoesNotExistInVersionException($"{property} exists in {version} only");
    }

    /// <summary>Version of the raw command; VXAce until the command is bound.</summary>
    protected StorageVersion Version => _version;

    private IParameterProvider Parameters => _parameters ??= CreateProvider(Raw, _version, _types);

    /// <summary>The parameters of the raw command, for the commands that expose them as they are.</summary>
    internal IParameterProvider GetParameterProvider() => Parameters;

    /// <summary>The type map to make objects inside the command from: the one of the storage, else the built-in one.</summary>
    protected TypeMap Map => _types ?? (_version == StorageVersion.MV ? TypeMap.Mv : TypeMap.VXAce);

    /// <summary>The type map of the storage; objects inside the command (a route, a sound) are made from it.</summary>
    internal TypeMap? Types
    {
        get => _types;
        set
        {
            _types = value;
            _parameters = null;
        }
    }

    /// <summary>
    /// Reads a parameter. <paramref name="property"/> is the name of the caller, which the compiler fills in: when the caller is a property that is
    /// marked with <see cref="DoesNotExistInVersionAttribute"/> and the maker of the command does not have it, this throws
    /// <see cref="DoesNotExistInVersionException"/>, so a property does not check that itself.
    /// </summary>
    protected T? Get<T>(int index, [System.Runtime.CompilerServices.CallerMemberName] string? property = null)
    {
        ThrowIfMissing(property);
        _lastIndex = index;

        return Parameters.GetParameter<T>(index);
    }

    private void ThrowIfMissing(string? property)
    {
        if (OptionalProperties.IsMissing(GetType(), property, IsDetached ? null : _version))
            throw new DoesNotExistInVersionException($"{property} does not exist in {_version}");
    }

    /// <summary>
    /// Writes a parameter. A tone, a color, a sound or a route is stored as the class of the type map. A command that
    /// is not in a container yet does not know its map, so it keeps the value as it is until it is put into one.
    /// </summary>
    protected void Set<T>(int index, T? value, [System.Runtime.CompilerServices.CallerMemberName] string? property = null)
    {
        ThrowIfMissing(property);
        _lastIndex = index;

        if (typeof(T).IsInterface && value != null && !IsDetached)
            value = (T)InMap(typeof(T), value);

        Parameters.SetParameter(index, value);
    }

    private object InMap(Type type, object value)
    {
        if (type == typeof(ITone)) return Tones.For((ITone)value, Map)!;
        if (type == typeof(IColor)) return Colors.For((IColor)value, Map)!;
        if (type == typeof(IMoveRoute)) return MoveRoutes.For((IMoveRoute)value, Map)!;
        if (type == typeof(IBGM)) return AudioFiles.For((IBGM)value, Map)!;
        if (type == typeof(IBGS)) return AudioFiles.For((IBGS)value, Map)!;
        if (type == typeof(IME)) return AudioFiles.For((IME)value, Map)!;
        if (type == typeof(ISE)) return AudioFiles.For((ISE)value, Map)!;

        return value;
    }

    /// <summary>
    /// Reads a parameter that is an object (a sound, a route...). The object stored in the raw command is returned, so
    /// changing it changes the command. A JSON object of MV is replaced by <typeparamref name="TJson"/> on the first read.
    /// </summary>
    protected T? GetEntity<T, TJson>(int index, [System.Runtime.CompilerServices.CallerMemberName] string? property = null) where T : class where TJson : class, T
    {
        ThrowIfMissing(property);
        _lastIndex = index;

        var value = Raw.Parameters?.Get(index);
        if (value is T entity)
            return entity;

        var converted = Parameters.GetParameter<TJson>(index);
        if (converted != null)
            Raw.Parameters![index] = converted;

        return converted;
    }

    /// <summary>
    /// <see cref="GetEntity{T,TJson}"/> for an object whose type is resolved by the type map, so the same type is
    /// used to read it from JSON.
    /// </summary>
    protected T? GetEntity<T>(int index, [System.Runtime.CompilerServices.CallerMemberName] string? property = null) where T : class => GetEntity<T, T>(index, property);

    /// <summary>True while the command has no container (and so no raw command of a file) yet.</summary>
    internal bool IsDetached => _raw == null || _raw is DetachedCommand;

    /// <summary>
    /// Makes the command a view over <paramref name="raw"/>, carrying over what was set in code.
    /// A <paramref name="loaded"/> command is created over a raw command of a file and is left as it is.
    /// </summary>
    internal void Bind(ICommand raw, StorageVersion version, bool loaded = false)
    {
        if (_raw is DetachedCommand detached && !ReferenceEquals(detached, raw))
        {
            raw.Indent = detached.Indent;
            raw.Parameters = detached.Parameters;
        }

        var madeInCode = !loaded && IsDetached;
        if (madeInCode)
            raw.Code = Code;

        _raw = raw;
        _version = version;
        _parameters = null;

        if (madeInCode)
        {
            if (!_copy)
                ApplyEditorDefaults(version);

            OnBinding(version);
            CompleteMadeInCode();
            OnMadeInCodeBound(version);
            _copy = false;
        }
    }

    /// <summary>What the editor gives to a new command (<see cref="EditorDefaultAttribute"/>), for the parameters that were not set in code.</summary>
    private void ApplyEditorDefaults(StorageVersion version)
    {
        foreach (var (property, defaults) in EditorDefaults.GetOrAdd(GetType(), FindEditorDefaults))
        {
            var attribute = defaults.FirstOrDefault(d => d.Versions.Contains(version)) ?? defaults.FirstOrDefault(d => d.Versions.Count == 0);
            if (attribute == null || !HasProperty(property.Name))
                continue;

            property.GetValue(this);   // the property reads its parameter, which tells which one it is
            if (IsParameterSet(_lastIndex))
                continue;

            property.SetValue(this, attribute.CreateValue(Map, property.PropertyType));
        }
    }

    private static (PropertyInfo, EditorDefaultBaseAttribute[])[] FindEditorDefaults(Type type) => ParameterProperties.GetOrAdd(type, FindParameterProperties)
        .Select(p => (Property: p, Defaults: p.GetCustomAttributes<EditorDefaultBaseAttribute>().ToArray()))
        .Where(p => p.Defaults.Length > 0)
        .Select(p => (p.Property, p.Defaults))
        .ToArray();

    /// <summary>
    /// Called when a command made in code gets its raw command, before its parameters are completed. A command that is not in a list has no maker and
    /// keeps what was set in code in one form for all makers (the neutral one, which the property says); here the parameters that are written
    /// differently by the makers are changed into the form of <paramref name="version"/>.
    /// </summary>
    protected virtual void OnBinding(StorageVersion version)
    {
    }

    /// <summary>
    /// True if the parameter was set in code before the command got its raw command. In <see cref="OnBinding"/> it tells what the editor's value
    /// is to be given to (the value of a new command) from what the user has set.
    /// </summary>
    protected bool IsParameterSet(int index) => Raw.Parameters != null && index < Raw.Parameters.Count && Raw.Parameters[index] != null;

    /// <summary>Called after a command made in code got its raw command, for parameters that depend on the version.</summary>
    protected virtual void OnMadeInCodeBound(StorageVersion version)
    {
    }

    /// <summary>A command made in code gets all of its parameters, like a command of a file has.</summary>
    private void CompleteMadeInCode()
    {
        foreach (var property in ParameterProperties.GetOrAdd(GetType(), FindParameterProperties))
        {
            if (!HasProperty(property.Name))
                continue;

            var value = property.GetValue(this);

            // a new command that plays a sound has an empty sound of the maker, as the editors give (not nothing)
            if (value == null && typeof(IAudioFile).IsAssignableFrom(property.PropertyType))
                value = NewEntityMethod.MakeGenericMethod(property.PropertyType).Invoke(Map, null);

            property.SetValue(this, value);
        }

        if (Raw.Parameters == null && (Raw.Code == 0 || Raw.Indent != null))
            Raw.Parameters = [];
    }

    private static PropertyInfo[] FindParameterProperties(Type type) => type
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0 && p.DeclaringType != typeof(NamedCommand)
                    && p.GetCustomAttribute<RawValueAttribute>() == null)
        .ToArray();

    /// <summary>False when the raw command was changed so that this class no longer describes it, e.g. its code.</summary>
    internal virtual bool MatchesRaw() => Raw.Code == Code;

    private static IParameterProvider CreateProvider(ICommand command, StorageVersion version, TypeMap? types) =>
        version == StorageVersion.MV ? new JsonParameterProvider(command, types) : new SimpleParameterProvider(command);

    public virtual string GetParametersDescription(EntityContext context, DumpOptions options) => "";

    public string GetPrefixSymbol(StorageVersion version)
    {
        switch (version)
        {
            case StorageVersion.MV:
                return "◆";
            case StorageVersion.VXAce:
                return "@>";
        }

        return "";
    }

    public string GetSecondaryPrefixSymbol(StorageVersion version)
    {
        switch (version)
        {
            case StorageVersion.MV:
                return ":";
            case StorageVersion.VXAce:
                return ": ";
        }

        return "";
    }

    protected virtual string GetStringPrefix(StorageVersion version)
    {
        return !IsSecondary ? GetPrefixSymbol(version) : GetSecondaryPrefixSymbol(version);
    }

    /// <summary>The pieces a command line is made of; <see cref="AsString"/> joins them.</summary>
    public readonly record struct CommandTextParts(
        string CodeLabel, string Indent, string Prefix, string Name, string Separator, string Parameters)
    {
        public override string ToString() => CodeLabel + Indent + Prefix + Name + Separator + Parameters;
    }

    public CommandTextParts GetTextParts(EntityContext context, DumpOptions options)
    {
        var indent = "";
        if (options.WithIndent && Indent > 0)
        {
            var whiteSpace = context.Version == StorageVersion.MV ? " " : "  ";

            indent = string.Concat(Enumerable.Repeat(whiteSpace, (int)Indent));
        }

        var name = GetCommandName(context.Version);
        var paramDesc = GetParametersDescription(context, options);

        var separator = "";
        if (!string.IsNullOrEmpty(paramDesc))
        {
            if (string.IsNullOrWhiteSpace(name) || !IsSecondary)
                separator = context.Version == StorageVersion.MV ? " : " : ": ";
            else
                separator = " ";
        }

        var codeLabel = options.AppendCommandCode ? $"({Code:D3}) " : "";

        return new CommandTextParts(codeLabel, indent, GetStringPrefix(context.Version), name, separator, paramDesc);
    }

    public string AsString(EntityContext context, DumpOptions options) => GetTextParts(context, options).ToString();
}
