using RPGMakerLibCoreV2.Data.Helpers.Dump;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.VXAce.Classes;
using RPGMakerLibCoreV2.Storages.Abstraction;
using RPGMakerLibCoreV2.Storages.Dump.TextSerializers;

namespace RPGMakerLibCoreV2.Storages.Dump;

public class VxAceYamlStorage : AbstractYamlStorage
{
    public VxAceYamlStorage(TypeMap? types = null) : base(new VxAceYamlSerializer(types))
    {
        AddSpecialSerializer<Script>(new ScriptSerializer());
    }

    protected override async Task _Dump(string path, Storage storage)
    {
        var vxStorage = (VXAceStorage)storage;

        Reporter?.SetStage(DumpStage.Progress);
        Reporter?.ResetTotal(16);

        await DumpHolder(path, nameof(vxStorage.Actors), vxStorage.Actors);
        await DumpHolder(path, nameof(vxStorage.Animations), vxStorage.Animations);
        await DumpHolder(path, nameof(vxStorage.Armors), vxStorage.Armors);
        await DumpHolder(path, nameof(vxStorage.Classes), vxStorage.Classes);
        await DumpHolder(path, nameof(vxStorage.CommonEvents), vxStorage.CommonEvents);
        await DumpHolder(path, nameof(vxStorage.Enemies), vxStorage.Enemies);
        await DumpHolder(path, nameof(vxStorage.Items), vxStorage.Items);
        await DumpHolder(path, nameof(vxStorage.Scripts), vxStorage.Scripts);
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
        var vxStorage = (VXAceStorage)storage;
        
        Reporter?.SetStage(DumpStage.Progress);
        Reporter?.ResetTotal(16);

        LoadHolder<IActor>(storage, path, nameof(storage.Actors), storage.Actors);
        LoadHolder<IAnimation>(storage, path, nameof(storage.Animations), storage.Animations);
        LoadHolder<IArmor>(storage, path, nameof(storage.Armors), storage.Armors);
        LoadHolder<IClass>(storage, path, nameof(storage.Classes), storage.Classes);
        LoadHolder<ICommonEvent>(storage, path, nameof(storage.CommonEvents), storage.CommonEvents);
        LoadHolder<IEnemy>(storage, path, nameof(storage.Enemies), storage.Enemies);
        LoadHolder<IItem>(storage, path, nameof(storage.Items), storage.Items);
        LoadHolder<Script>(storage, path, nameof(vxStorage.Scripts), vxStorage.Scripts);
        LoadHolder<ISkill>(storage, path, nameof(storage.Skills), storage.Skills);
        LoadHolder<IState>(storage, path, nameof(storage.States), storage.States);
        LoadHolder<ISystem>(storage, path, nameof(storage.System), storage.System);
        LoadHolder<IMapInfo>(storage, path, nameof(storage.MapInfos), storage.MapInfos);
        LoadHolder<ITileSet>(storage, path, nameof(storage.Tilesets), storage.Tilesets);
        LoadHolder<ITroop>(storage, path, nameof(storage.Troops), storage.Troops);
        LoadHolder<IWeapon>(storage, path, nameof(storage.Weapons), storage.Weapons);
        
        LoadMaps(path, storage);
        
        Reporter?.SetStage(DumpStage.Finished);
    }

    protected override string GetExtension<T>()
    {
        if (typeof(T) == typeof(Script))
            return "txt";
        return base.GetExtension<T>();
    }
}