using RPGMakerLibCoreV2.Data.Helpers.Dump;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.VXAce.Classes;
using RPGMakerLibCoreV2.Storages.Abstraction;
using RPGMakerLibCoreV2.Storages.Dump.TextSerializers;

namespace RPGMakerLibCoreV2.Storages.Dump;

public class VxAceYamlStorage : AbstractYamlStorage
{
    public VxAceYamlStorage() : base(new VxAceYamlSerializer())
    {
        AddSpecialSerializer<Script>(new ScriptSerializer());
    }

    protected override void _Dump(string path, Storage storage)
    {
        var vxStorage = (VXAceStorage)storage;

        Reporter?.SetStage(DumpStage.Init);
        Reporter?.Report();
        
        Reporter?.SetStage(DumpStage.Progress);
        
        Reporter?.SetGlobalTotal(16);

        DumpHolder(path, nameof(vxStorage.Actors), vxStorage.Actors);
        DumpHolder(path, nameof(vxStorage.Animations), vxStorage.Animations);
        DumpHolder(path, nameof(vxStorage.Armors), vxStorage.Armors);
        DumpHolder(path, nameof(vxStorage.Classes), vxStorage.Classes);
        DumpHolder(path, nameof(vxStorage.CommonEvents), vxStorage.CommonEvents);
        DumpHolder(path, nameof(vxStorage.Enemies), vxStorage.Enemies);
        DumpHolder(path, nameof(vxStorage.Items), vxStorage.Items);
        DumpHolder(path, nameof(vxStorage.Scripts), vxStorage.Scripts);
        DumpHolder(path, nameof(vxStorage.Skills), vxStorage.Skills);
        DumpHolder(path, nameof(vxStorage.States), vxStorage.States);
        DumpHolder(path, nameof(vxStorage.System), vxStorage.System);
        DumpHolder(path, nameof(vxStorage.MapInfos), vxStorage.MapInfos);
        DumpHolder(path, nameof(vxStorage.Tilesets), vxStorage.Tilesets);
        DumpHolder(path, nameof(vxStorage.Troops), vxStorage.Troops);
        DumpHolder(path, nameof(vxStorage.Weapons), vxStorage.Weapons);
        
        DumpMaps(path, vxStorage);
        
        Reporter?.SetStage(DumpStage.Finished);
    }

    protected override void _Load(string path, Storage storage)
    {
        var vxStorage = (VXAceStorage)storage;
        
        Reporter?.SetStage(DumpStage.Init);
        Reporter?.Report();
        
        Reporter?.SetStage(DumpStage.Progress);
        
        Reporter?.SetGlobalTotal(16);

        LoadHolder<Actor, IActor>(path, nameof(storage.Actors), storage.Actors);
        LoadHolder<Animation, IAnimation>(path, nameof(storage.Animations), storage.Animations);
        LoadHolder<Armor, IArmor>(path, nameof(storage.Armors), storage.Armors);
        LoadHolder<Class, IClass>(path, nameof(storage.Classes), storage.Classes);
        LoadHolder<CommonEvent, ICommonEvent>(path, nameof(storage.CommonEvents), storage.CommonEvents);
        LoadHolder<Enemy, IEnemy>(path, nameof(storage.Enemies), storage.Enemies);
        LoadHolder<Item, IItem>(path, nameof(storage.Items), storage.Items);
        LoadHolder<Script, Script>(path, nameof(vxStorage.Scripts), vxStorage.Scripts);
        LoadHolder<Skill, ISkill>(path, nameof(storage.Skills), storage.Skills);
        LoadHolder<State, IState>(path, nameof(storage.States), storage.States);
        LoadHolder<Data.VXAce.Classes.System, ISystem>(path, nameof(storage.System), storage.System);
        LoadHolder<MapInfo, IMapInfo>(path, nameof(storage.MapInfos), storage.MapInfos);
        LoadHolder<Tileset, ITileSet>(path, nameof(storage.Tilesets), storage.Tilesets);
        LoadHolder<Troop, ITroop>(path, nameof(storage.Troops), storage.Troops);
        LoadHolder<Weapon, IWeapon>(path, nameof(storage.Weapons), storage.Weapons);
        
        LoadMaps<Map>(path, storage);
        
        Reporter?.SetStage(DumpStage.Finished);
    }

    protected override string GetExtension<T>()
    {
        if (typeof(T) == typeof(Script))
            return "txt";
        return base.GetExtension<T>();
    }
}