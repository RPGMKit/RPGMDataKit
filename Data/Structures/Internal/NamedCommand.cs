using System.Collections.Concurrent;
using System.Reflection;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

/// <summary>
/// A command with named properties. It is a view over a raw command: properties read and write the raw command's
/// parameters directly, so there is nothing to unpack, pack or keep in sync.
/// </summary>
public abstract class NamedCommand
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> ParameterProperties = new();

    private ICommand? _raw;
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

    protected T? Get<T>(int index) => Parameters.GetParameter<T>(index);

    /// <summary>
    /// Writes a parameter. A tone, a color, a sound or a route is stored as the class of the type map. A command that
    /// is not in a container yet does not know its map, so it keeps the value as it is until it is put into one.
    /// </summary>
    protected void Set<T>(int index, T? value)
    {
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
    protected T? GetEntity<T, TJson>(int index) where T : class where TJson : class, T
    {
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
    protected T? GetEntity<T>(int index) where T : class => GetEntity<T, T>(index);

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
            CompleteMadeInCode();
            OnMadeInCodeBound(version);
        }
    }

    /// <summary>Called after a command made in code got its raw command, for parameters that depend on the version.</summary>
    protected virtual void OnMadeInCodeBound(StorageVersion version)
    {
    }

    /// <summary>A command made in code gets all of its parameters, like a command of a file has.</summary>
    private void CompleteMadeInCode()
    {
        foreach (var property in ParameterProperties.GetOrAdd(GetType(), FindParameterProperties))
            property.SetValue(this, property.GetValue(this));

        if (Raw.Parameters == null && (Raw.Code == 0 || Raw.Indent != null))
            Raw.Parameters = [];
    }

    private static PropertyInfo[] FindParameterProperties(Type type) => type
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0 && p.DeclaringType != typeof(NamedCommand))
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

    public string AsString(EntityContext context, DumpOptions options)
    {
        string str = "";
        if (options.WithIndent && Indent > 0)
        {
            var whiteSpace = context.Version == StorageVersion.MV ? " " : "  ";

            str = string.Concat(Enumerable.Repeat(whiteSpace, (int)Indent));
        }

        str += GetStringPrefix(context.Version);

        var name = GetCommandName(context.Version);
        str += name;

        var paramDesc = GetParametersDescription(context, options);

        if (!string.IsNullOrEmpty(paramDesc))
        {
            if (string.IsNullOrWhiteSpace(name) || !IsSecondary)
                str += context.Version == StorageVersion.MV ? " : " : ": ";
            else
                str += " ";

            str += paramDesc;
        }

        if (options.AppendCommandCode)
            str = $"({Code:D3}) {str}";

        // str += $"Indent: " + (Indent != null ? Indent : "null");

        return str;
    }
}
