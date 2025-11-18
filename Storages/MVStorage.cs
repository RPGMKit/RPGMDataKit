using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Storages;

public class MVStorage : Storage
{
    public MVStorage(string projectPath, string dataDirectory = "data") : base(projectPath, dataDirectory)
    {
        Version = StorageVersion.MV;
    }

    protected override string MapPath(int index) => $"Map{index:D3}.json";

    protected override void Init()
    {
        Actors = LoadListHolder<Actor, IActor>("Actors.json");
        Animations = LoadListHolder<Animation, IAnimation>("Animations.json");
        Armors = LoadListHolder<Armor, IArmor>("Armors.json");
        Classes = LoadListHolder<Class, IClass>("Classes.json");
        CommonEvents = LoadListHolder<CommonEvent, ICommonEvent>("CommonEvents.json");
        Enemies = LoadListHolder<Enemy, IEnemy>("Enemies.json");
        Items = LoadListHolder<Item, IItem>("Items.json");
        Maps = new(this, MapPath, LoadData<Map>);
        MapInfos = LoadListHolder<MapInfo, IMapInfo>("MapInfos.json");
        Skills = LoadListHolder<Skill, ISkill>("Skills.json");
        States = LoadListHolder<State, IState>("States.json");
        System = LoadHolder<Data.MV.Classes.System, ISystem>("System.json");
        Tilesets = LoadListHolder<Tileset, ITileSet>("Tilesets.json");
        Troops = LoadListHolder<Troop, ITroop>("Troops.json");
        Weapons = LoadListHolder<Weapon, IWeapon>("Weapons.json");
    }

    public override string DataPath(string dataObject)
    {
        return Path.Combine(DataDirectory, dataObject);
    }

    public override T LoadData<T>(string dataFile)
    {
        var path = Path.Combine(ProjectPath, DataDirectory, dataFile);
        if (!File.Exists(path))
            throw new FileNotFoundException(path);

        var contents = File.ReadAllText(path);
        if (string.IsNullOrWhiteSpace(contents))
            throw new FileLoadException(path);

        return JsonConvert.DeserializeObject<T>(contents);
    }

    public override void SaveData<T>(string dataFile, T data)
    {
        var path = Path.Combine(ProjectPath, DataDirectory, dataFile);
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
