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

    protected override void Init()
    {
        Animations = new(this, () =>
        {
            var path = DataPath("Animations.json");

            return new()
            {
                Path = path,
                Data = LoadData<LookupList<Animation>>(path)
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
        
        System = new(this, () =>
        {
            var path = DataPath("System.json");

            return new()
            {
                Path = path,
                Data = LoadData<Data.MV.Classes.System>(path)
            };
        });
        
        Maps = new(this, LoadData<Map>);
        
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
        Animations.Save();
        CommonEvents.Save();
        System.Save();
        Maps.Save();
        MapInfos.Save();
        Tilesets.Save();
    }
}
