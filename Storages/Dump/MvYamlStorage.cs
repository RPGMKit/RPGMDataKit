using RPGMakerLibCoreV2.Data.Helpers.Dump;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages.Abstraction;
using RPGMakerLibCoreV2.Storages.Dump.TextSerializers;

namespace RPGMakerLibCoreV2.Storages.Dump;

public class MvYamlStorage : AbstractYamlStorage
{
    public MvYamlStorage() : base(new MvYamlSerializer())
    {
    }

    protected override async Task _Dump(string path, Storage storage)
    {
        var vxStorage = (MVStorage)storage;

        Reporter?.SetStage(DumpStage.Progress);
        Reporter?.ResetTotal(15);

        await DumpHolder(path, nameof(vxStorage.Actors), vxStorage.Actors);
        await DumpHolder(path, nameof(vxStorage.Animations), vxStorage.Animations);
        await DumpHolder(path, nameof(vxStorage.Armors), vxStorage.Armors);
        await DumpHolder(path, nameof(vxStorage.Classes), vxStorage.Classes);
        await DumpHolder(path, nameof(vxStorage.CommonEvents), vxStorage.CommonEvents);
        await DumpHolder(path, nameof(vxStorage.Enemies), vxStorage.Enemies);
        await DumpHolder(path, nameof(vxStorage.Items), vxStorage.Items);
        await DumpHolder(path, nameof(vxStorage.Skills), vxStorage.Skills);
        await DumpHolder(path, nameof(vxStorage.States), vxStorage.States);
        await DumpHolder(path, nameof(vxStorage.System), vxStorage.System);
        await DumpHolder(path, nameof(vxStorage.MapInfos), vxStorage.MapInfos);
        await DumpHolder(path, nameof(vxStorage.Tilesets), vxStorage.Tilesets);
        await DumpHolder(path, nameof(vxStorage.Troops), vxStorage.Troops);
        await DumpHolder(path, nameof(vxStorage.Weapons), vxStorage.Weapons);
        
        await DumpMaps(path, vxStorage);
        
        Reporter?.SetStage(DumpStage.Finished);
    }

    protected override void _Load(string path, Storage storage)
    {
        Reporter?.SetStage(DumpStage.Progress);
        Reporter?.ResetTotal(15);

        LoadHolder<Actor, IActor>(path, nameof(storage.Actors), storage.Actors);
        LoadHolder<Animation, IAnimation>(path, nameof(storage.Animations), storage.Animations);
        LoadHolder<Armor, IArmor>(path, nameof(storage.Armors), storage.Armors);
        LoadHolder<Class, IClass>(path, nameof(storage.Classes), storage.Classes);
        LoadHolder<CommonEvent, ICommonEvent>(path, nameof(storage.CommonEvents), storage.CommonEvents);
        LoadHolder<Enemy, IEnemy>(path, nameof(storage.Enemies), storage.Enemies);
        LoadHolder<Item, IItem>(path, nameof(storage.Items), storage.Items);
        LoadHolder<Skill, ISkill>(path, nameof(storage.Skills), storage.Skills);
        LoadHolder<State, IState>(path, nameof(storage.States), storage.States);
        LoadHolder<Data.MV.Classes.System, ISystem>(path, nameof(storage.System), storage.System);
        LoadHolder<MapInfo, IMapInfo>(path, nameof(storage.MapInfos), storage.MapInfos);
        LoadHolder<Tileset, ITileSet>(path, nameof(storage.Tilesets), storage.Tilesets);
        LoadHolder<Troop, ITroop>(path, nameof(storage.Troops), storage.Troops);
        LoadHolder<Weapon, IWeapon>(path, nameof(storage.Weapons), storage.Weapons);
        
        LoadMaps<Map>(path, storage);
        
        Reporter?.SetStage(DumpStage.Finished);
    }
}
