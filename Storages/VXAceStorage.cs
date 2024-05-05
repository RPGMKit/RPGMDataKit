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

    protected override void Init()
    {
        Animations = new(this, () =>
        {
            var path = DataPath("Animations.rvdata2");

            return new()
            {
                Path = path,
                Data = LoadData<List<Animation>>(path)
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
        
        System = new(this, () =>
        {
            var path = DataPath("System.rvdata2");

            return new()
            {
                Path = path,
                Data = LoadData<Data.VXAce.Classes.System>(path)
            };
        });
        
        Maps = new(this, LoadData<Map>);
        
        MapInfos = new(this, () =>
        {
            var path = DataPath("MapInfos.rvdata2");

            return new()
            {
                Path = path,
                Data = LoadData<Dictionary<int, MapInfo>>(path).Values
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