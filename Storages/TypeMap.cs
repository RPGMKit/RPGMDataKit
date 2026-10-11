using System.Linq.Expressions;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages.Serialization;

namespace RPGMakerLibCoreV2.Storages;

/// <summary>Maps entity interfaces to the concrete types of one maker version.</summary>
public class TypeMap
{
    private static readonly string InterfacesNamespace = typeof(IActor).Namespace!;

    private readonly Dictionary<Type, Type> _map = new();
    private readonly Dictionary<Type, List<Type>> _ambiguous = new();
    private readonly Dictionary<Type, Delegate> _factories = new();
    private JsonSerializer? _json;
    private bool _readOnly;

    /// <summary>Built-in map for MV. Read-only: use <see cref="Clone"/> to customize.</summary>
    public static TypeMap Mv { get; } = FromNamespace("RPGMakerLibCoreV2.Data.MV.Classes");

    /// <summary>Built-in map for VXAce. Read-only: use <see cref="Clone"/> to customize.</summary>
    public static TypeMap VXAce { get; } = FromNamespace("RPGMakerLibCoreV2.Data.VXAce.Classes");

    /// <summary>Registers (or replaces) the implementation of an entity interface.</summary>
    public TypeMap Add<TInterface, TConcrete>() where TConcrete : TInterface
    {
        if (_readOnly)
            throw new InvalidOperationException("Built-in type maps are read-only, use Clone() first");

        _map[typeof(TInterface)] = typeof(TConcrete);
        _ambiguous.Remove(typeof(TInterface));
        _factories.Clear();
        _json = null;

        return this;
    }

    /// <summary>Returns an editable copy.</summary>
    public TypeMap Clone()
    {
        var copy = new TypeMap();
        foreach (var (key, value) in _map)
            copy._map[key] = value;

        foreach (var (key, value) in _ambiguous)
            copy._ambiguous[key] = new List<Type>(value);

        return copy;
    }

    /// <summary>
    /// Returns the registered concrete type, or <paramref name="type"/> itself when it is not an entity interface.
    /// </summary>
    public Type Resolve(Type type)
    {
        if (_ambiguous.TryGetValue(type, out var candidates))
            throw new InvalidOperationException(
                $"{type.Name} is implemented by several types: {string.Join(", ", candidates.Select(c => c.FullName))}. " +
                $"Pick one with TypeMap.Add<{type.Name}, TConcrete>()");

        var concrete = _map.GetValueOrDefault(type, type);
        if (concrete.IsInterface && concrete.Namespace == InterfacesNamespace)
            throw new InvalidOperationException(
                $"No implementation registered for {type.Name}. Register one with TypeMap.Add<{type.Name}, TConcrete>()");

        return concrete;
    }

    /// <summary>Compiled parameterless constructor of the implementation registered for <typeparamref name="T"/>.</summary>
    public Func<T> Factory<T>() where T : class
    {
        lock (_factories)
        {
            if (!_factories.TryGetValue(typeof(T), out var factory))
                _factories[typeof(T)] = factory = Expression.Lambda<Func<T>>(Expression.New(Resolve(typeof(T)))).Compile();

            return (Func<T>)factory;
        }
    }

    /// <summary>
    /// The JSON serializer that makes entities from this map. Its context holds the map, so the containers of
    /// commands that it creates know it too (see <see cref="Data.Structures.Internal.JsonCommandContainer{TCommand}"/>).
    /// </summary>
    internal JsonSerializer JsonSerializer => _json ??= JsonSerializer.Create(CreateJsonSettings());

    internal JsonSerializerSettings CreateJsonSettings() => new()
    {
        ContractResolver = new TypeMapContractResolver(this),
        Context = new StreamingContext(StreamingContextStates.Other, this),
    };

    /// <summary>
    /// Maps every entity interface to the only non-abstract class that declares it in
    /// <paramref name="classesNamespace"/> (including nested namespaces).
    /// </summary>
    private static TypeMap FromNamespace(string classesNamespace)
    {
        var candidates = new Dictionary<Type, List<Type>>();

        var classes = typeof(TypeMap).Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } &&
                        t.Namespace != null &&
                        (t.Namespace == classesNamespace || t.Namespace.StartsWith(classesNamespace + ".")));

        foreach (var type in classes)
        {
            foreach (var entityInterface in DeclaredInterfaces(type).Where(i => i.Namespace == InterfacesNamespace))
            {
                if (!candidates.TryGetValue(entityInterface, out var list))
                    candidates[entityInterface] = list = new();

                list.Add(type);
            }
        }

        var map = new TypeMap { _readOnly = true };
        foreach (var (entityInterface, types) in candidates)
        {
            if (types.Count == 1)
                map._map[entityInterface] = types[0];
            else
                map._ambiguous[entityInterface] = types;
        }

        return map;
    }

    /// <summary>Interfaces the type declares itself, without those inherited from its base type or other interfaces.</summary>
    private static IEnumerable<Type> DeclaredInterfaces(Type type)
    {
        var all = type.GetInterfaces();
        var inherited = all.SelectMany(i => i.GetInterfaces())
            .Concat(type.BaseType?.GetInterfaces() ?? []);

        return all.Except(inherited);
    }
}
