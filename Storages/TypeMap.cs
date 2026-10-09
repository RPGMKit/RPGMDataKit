using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Storages;

/// <summary>Maps entity interfaces to the concrete types of one maker version.</summary>
public class TypeMap
{
    private readonly Dictionary<Type, Type> _map = new();

    public TypeMap Add<TInterface, TConcrete>() where TConcrete : TInterface
    {
        _map[typeof(TInterface)] = typeof(TConcrete);
        return this;
    }

    /// <summary>Returns the registered concrete type, or <paramref name="type"/> itself when it has no mapping.</summary>
    public Type Resolve(Type type) => _map.GetValueOrDefault(type, type);

    public static TypeMap Mv { get; } = new TypeMap()
        .Add<IActor, Data.MV.Classes.Actor>()
        .Add<IAnimation, Data.MV.Classes.Animation>()
        .Add<IArmor, Data.MV.Classes.Armor>()
        .Add<IClass, Data.MV.Classes.Class>()
        .Add<ICommonEvent, Data.MV.Classes.CommonEvent>()
        .Add<IEnemy, Data.MV.Classes.Enemy>()
        .Add<IItem, Data.MV.Classes.Item>()
        .Add<IMap, Data.MV.Classes.Map>()
        .Add<IMapInfo, Data.MV.Classes.MapInfo>()
        .Add<ISkill, Data.MV.Classes.Skill>()
        .Add<IState, Data.MV.Classes.State>()
        .Add<ISystem, Data.MV.Classes.System>()
        .Add<ITileSet, Data.MV.Classes.Tileset>()
        .Add<ITroop, Data.MV.Classes.Troop>()
        .Add<IWeapon, Data.MV.Classes.Weapon>();

    public static TypeMap VXAce { get; } = new TypeMap()
        .Add<IActor, Data.VXAce.Classes.Actor>()
        .Add<IAnimation, Data.VXAce.Classes.Animation>()
        .Add<IArmor, Data.VXAce.Classes.Armor>()
        .Add<IClass, Data.VXAce.Classes.Class>()
        .Add<ICommonEvent, Data.VXAce.Classes.CommonEvent>()
        .Add<IEnemy, Data.VXAce.Classes.Enemy>()
        .Add<IItem, Data.VXAce.Classes.Item>()
        .Add<IMap, Data.VXAce.Classes.Map>()
        .Add<IMapInfo, Data.VXAce.Classes.MapInfo>()
        .Add<ISkill, Data.VXAce.Classes.Skill>()
        .Add<IState, Data.VXAce.Classes.State>()
        .Add<ISystem, Data.VXAce.Classes.System>()
        .Add<ITileSet, Data.VXAce.Classes.Tileset>()
        .Add<ITroop, Data.VXAce.Classes.Troop>()
        .Add<IWeapon, Data.VXAce.Classes.Weapon>();
}
