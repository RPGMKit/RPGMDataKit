using System.Reflection;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Storages;
using RPGMDataKit.Storages.Serialization;

namespace RPGMDataKit.Tests;

/// <summary>The two makers that the library supports, with what the tests need to treat them the same way.</summary>
public sealed record Version(string Name, TypeMap Types, IDataSerializer Serializer)
{
    public static readonly Version Mv = new("MV", TypeMap.Mv, new JsonDataSerializer(TypeMap.Mv));

    public static readonly Version VXAce = new("VXAce", TypeMap.VXAce, new RubyMarshalDataSerializer());

    public static IEnumerable<Version> All => [Mv, VXAce];

    public override string ToString() => Name;

    /// <summary>A new entity as the factory of the library makes it (<see cref="TypeMap.New{T}"/>).</summary>
    public T New<T>() where T : class => Types.New<T>();

    /// <summary>The same by the interface type that is known at run time only.</summary>
    public object New(Type entityInterface) =>
        typeof(TypeMap).GetMethod(nameof(TypeMap.New))!.MakeGenericMethod(entityInterface).Invoke(Types, null)!;

    /// <summary>The entity interfaces whose implementation in this version knows the values of a new entity.</summary>
    public IEnumerable<Type> FactoryInterfaces() =>
        typeof(IActor).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.Namespace == typeof(IActor).Namespace)
            .Where(t =>
            {
                try
                {
                    return typeof(INewEntity).IsAssignableFrom(Types.Resolve(t)) && !Types.Resolve(t).IsAbstract;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            })
            .OrderBy(t => t.Name);

    public static IEnumerable<object[]> FactoryCases() =>
        All.SelectMany(v => v.FactoryInterfaces().Select(i => new object[] { v, i }));

    public Type Concrete(Type entityInterface) => Types.Resolve(entityInterface);

    /// <summary>Loads a file as the concrete type of the interface.</summary>
    public object Load(Type entityInterface, string path) =>
        Serializer.GetType().GetMethod(nameof(IDataSerializer.Load))!.MakeGenericMethod(Concrete(entityInterface))
            .Invoke(Serializer, [path])!;

    public void Save(object entity, string path) =>
        Serializer.GetType().GetMethod(nameof(IDataSerializer.Save))!.MakeGenericMethod(entity.GetType())
            .Invoke(Serializer, [path, entity]);
}
