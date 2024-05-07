using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Storages;

public class MVStorage : Storage
{
    public MVStorage(string projectPath) : base(projectPath)
    {
        Version = StorageVersion.MV;
    }

    protected override string MapPath(int index) => DataPath($"Map{index:D3}.json");

    protected override void Init()
    {
        Actors = new(this, DataPath("Actors.json"), LoadData<LookupList<Actor>>);
        Animations = new(this, DataPath("Animations.json"), LoadData<LookupList<Animation>>);
        Armors = new(this, DataPath("Armors.json"), LoadData<LookupList<Armor>>);
        Classes = new(this, DataPath("Classes.json"), LoadData<LookupList<Class>>);
        CommonEvents = new(this, DataPath("CommonEvents.json"), LoadData<LookupList<CommonEvent>>);
        Enemies = new(this, DataPath("Enemies.json"), LoadData<LookupList<Enemy>>);
        Items = new(this, DataPath("Items.json"), LoadData<LookupList<Item>>);
        Skills = new(this, DataPath("Skills.json"), LoadData<LookupList<Skill>>);
        States = new(this, DataPath("States.json"), LoadData<LookupList<State>>);
        System = new(this, DataPath("System.json"), LoadData<Data.MV.Classes.System>);
        Maps = new(this, MapPath, LoadData<Map>);
        MapInfos = new(this, DataPath("MapInfos.json"), LoadData<LookupList<MapInfo>>);
        Tilesets = new(this, DataPath("TileSets.json"), LoadData<LookupList<Tileset>>);
        Troops = new(this, DataPath("Troops.json"), LoadData<LookupList<Troop>>);
        Weapons = new(this, DataPath("Weapons.json"), LoadData<LookupList<Weapon>>);
    }

    public override string DataPath(string dataObject)
    {
        return Path.Combine("data", dataObject);
    }
    
    public override T LoadData<T>(string relativePath)
    {
        var path = Path.Combine(ProjectPath, relativePath);
        if (!File.Exists(path))
            throw new FileNotFoundException(path);

        var contents = File.ReadAllText(path);
        if (string.IsNullOrWhiteSpace(contents))
            throw new FileLoadException(path);

        return JsonConvert.DeserializeObject<T>(contents);
    }

    public override void SaveData<T>(string relativePath, T data)
    {
        var path = Path.Combine(ProjectPath, relativePath);
        File.WriteAllText(path, JsonConvert.SerializeObject(data));
    }

    public override void SaveAll()
    {
        Actors.Save();
        Animations.Save();
        Armors.Save();
        Classes.Save();
        CommonEvents.Save();
        Items.Save();
        Skills.Save();
        States.Save();
        System.Save();
        Maps.Save();
        MapInfos.Save();
        Tilesets.Save();
        Weapons.Save();
    }
}
