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
        Actors = new(this, () =>
        {
            var path = DataPath("Actors.json");

            return new()
            {
                Path = path,
                Data = LoadData<LookupList<Actor>>(path)
            };
        });
        
        Animations = new(this, () =>
        {
            var path = DataPath("Animations.json");

            return new()
            {
                Path = path,
                Data = LoadData<LookupList<Animation>>(path)
            };
        });
        
        Armors = new(this, () =>
        {
            var path = DataPath("Armors.json");

            return new()
            {
                Path = path,
                Data = LoadData<LookupList<Armor>>(path)
            };
        });
        
        Classes = new(this, () =>
        {
            var path = DataPath("Classes.json");

            return new()
            {
                Path = path,
                Data = LoadData<List<Class>>(path)
            };
        });
        
        CommonEvents = new(this, () =>
        {
            var path = DataPath("CommonEvents.json");

            return new()
            {
                Path = path,
                Data = LoadData<LookupList<CommonEvent>>(path)
            };
        });
        
        Items = new(this, () =>
        {
            var path = DataPath("Items.json");

            return new()
            {
                Path = path,
                Data = LoadData<LookupList<Item>>(path)
            };
        });
        
        Skills = new(this, () =>
        {
            var path = DataPath("Skills.json");

            return new()
            {
                Path = path,
                Data = LoadData<LookupList<Skill>>(path)
            };
        });
        
        States = new(this, () =>
        {
            var path = DataPath("States.json");

            return new()
            {
                Path = path,
                Data = LoadData<LookupList<State>>(path)
            };
        });
        
        System = new(this, () =>
        {
            var path = DataPath("System.json");

            return new()
            {
                Path = path,
                Data = LoadData<Data.MV.Classes.System>(path)
            };
        });
        
        Maps = new(this, (mapIndex) =>
        {
            var path = MapPath(mapIndex);
            return new()
            {
                Path = path,
                Data = LoadData<Map>(path)
            };
        });
        
        MapInfos = new(this, () =>
        {
            var path = DataPath("MapInfos.json");

            return new()
            {
                Path = path,
                Data = LoadData<LookupList<MapInfo>>(path)
            };
        });
        
        Tilesets = new(this, () =>
        {
            var path = DataPath("TileSets.json");

            return new()
            {
                Path = path,
                Data = LoadData<LookupList<Tileset>>(path)
            };
        });
        
        Weapons = new(this, () =>
        {
            var path = DataPath("Weapons.json");

            return new()
            {
                Path = path,
                Data = LoadData<LookupList<Weapon>>(path)
            };
        });
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
