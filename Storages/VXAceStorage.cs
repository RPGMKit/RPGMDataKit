using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.VXAce.Classes;
using RPGMakerLibCoreV2.Storages.Abstraction;
using RubyMarshalCS;

namespace RPGMakerLibCoreV2.Storages;

public class VXAceStorage : Storage
{
    public VXAceStorage(string projectPath) : base(projectPath)
    {
        Version = StorageVersion.VXAce;
    }

    protected override string MapPath(int index) => DataPath($"Map{index:D3}.rvdata2");
    
    protected override void Init()
    {
        Actors = new(this, () =>
        {
            var path = DataPath("Actors.rvdata2");

            return new()
            {
                Path = path,
                Data = LoadData<LookupList<Actor>>(path)
            };
        });

        Animations = new(this, () =>
        {
            var path = DataPath("Animations.rvdata2");

            return new()
            {
                Path = path,
                Data = LoadData<List<Animation>>(path)
            };
        });

        Armors = new(this, () =>
        {
            var path = DataPath("Armors.rvdata2");

            return new()
            {
                Path = path,
                Data = LoadData<LookupList<Armor>>(path)
            };
        });
        
        Classes = new(this, () =>
        {
            var path = DataPath("Classes.rvdata2");

            return new()
            {
                Path = path,
                Data = LoadData<List<Class>>(path)
            };
        });
        
        CommonEvents = new(this, () =>
        {
            var path = DataPath("CommonEvents.rvdata2");

            return new()
            {
                Path = path,
                Data = LoadData<List<CommonEvent>>(path)
            };
        });
        
        Items = new(this, () =>
        {
            var path = DataPath("Items.rvdata2");

            return new()
            {
                Path = path,
                Data = LoadData<List<Item>>(path)
            };
        });
        
        Skills = new(this, () =>
        {
            var path = DataPath("Skills.rvdata2");

            return new()
            {
                Path = path,
                Data = LoadData<List<Skill>>(path)
            };
        });
        
        States = new(this, () =>
        {
            var path = DataPath("States.rvdata2");

            return new()
            {
                Path = path,
                Data = LoadData<List<State>>(path)
            };
        });
        
        System = new(this, () =>
        {
            var path = DataPath("System.rvdata2");

            return new()
            {
                Path = path,
                Data = LoadData<Data.VXAce.Classes.System>(path)
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
            var path = DataPath("MapInfos.rvdata2");


            var data = LoadData<Dictionary<int, MapInfo>>(path);

            LookupList<MapInfo> remapped = new();
            foreach (var (id,info) in data)
                remapped.Set(id, info);

            return new()
            {
                Path = path,
                Data = remapped
            };
        });
        
        Tilesets = new(this, () =>
        {
            var path = DataPath("Tilesets.rvdata2");

            return new()
            {
                Path = path,
                Data = LoadData<LookupList<TileSet>>(path)
            };
        });
        
        Weapons = new(this, () =>
        {
            var path = DataPath("Weapons.rvdata2");

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
        
        return RubyMarshal.Load<T>(DataPath(path));
    }

    public override void SaveData<T>(string relativePath, T data)
    {
        throw new NotImplementedException();
    }

    public override void SaveAll()
    {
        throw new NotImplementedException();
    }
}