using RPGMakerLibCoreV2.Data;
using RPGMakerLibCoreV2.Data.Helpers.Dump;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages.Abstraction;
using YamlDotNet.Serialization;

namespace RPGMakerLibCoreV2.Storages.Dump;

public abstract class AbstractYamlDumper
{
    public DumpProgressReporter? Reporter { get; set; }

    private bool NeedCancel => Reporter?.NeedCancel ?? false;
    
    public void Dump(string path, Storage storage)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist");

        _Dump(path, storage);
    }

    protected abstract void _Dump(string path, Storage storage);

    public void Load(string path, Storage storage)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist");

        _Load(path, storage);
    }

    protected abstract void _Load(string path, Storage storage);
    
    protected void DumpMaps(string path, Storage storage)
    {
        if (NeedCancel)
            return;
        
        Reporter?.ResetTotal(storage.MapInfos.Get().Count);
        
        foreach (var (mapId, _) in storage.MapInfos.Get())
        {
            if (NeedCancel)
                break;

            using var rp = Reporter?.Report(nameof(storage.Maps), mapId);
            
            var map = storage.Maps.Get(mapId);
            if (map != null)
                DumpEntity(Path.Combine(path, nameof(storage.Maps)), mapId.ToString(), map);
        }
    }
    
    protected void LoadMaps<T>(string path, Storage storage) where T: IMap
    {
        if (NeedCancel)
            return;
        
        Reporter?.ResetTotal(storage.MapInfos.Get().Count);

        var mapsDirectory = Path.Combine(path, nameof(storage.Maps));
        if (!Directory.Exists(mapsDirectory))
            return;
        
        var mapFiles = Directory.GetFiles(mapsDirectory, "*.yaml");
        
        Reporter?.ResetTotal(mapFiles.Length);
        
        foreach (var mapFile in mapFiles)
        {
            if (NeedCancel)
                break;
            
            if (!int.TryParse(Path.GetFileNameWithoutExtension(mapFile), out var id))
                continue;

            using var rp = Reporter?.Report(nameof(storage.Maps), id);
            
            var map = LoadEntity <T>(mapFile);
            
            storage.Maps.Set(id, map);
        }
    }

    protected void DumpHolder<T>(string path, string entityName, DataLookupHolder<T> holder)
    {
        if (NeedCancel)
            return;
        
        using var rp1 = Reporter?.ReportGlobal();
        
        path = Path.Combine(path, entityName);

        Reporter?.ResetTotal(holder.Get().Count);

        foreach (var (i, e) in holder.Get())
        {
            if (NeedCancel)
                break;
            
            using var rp2 = Reporter?.Report(entityName, i);

            if (e != null)
                DumpEntity(path, i.ToString(), e);
        }
    }

    protected void DumpHolder<T>(string path, string entityName, DataHolder<T> holder)
    {
        if (NeedCancel)
            return;

        using var rp1 = Reporter?.ReportGlobal();
        
        Reporter?.ResetTotal(1);
        using var rp2 = Reporter?.Report(entityName);
        DumpEntity(path, entityName, holder.Get());
    }

    protected void LoadHolder<T, TV>(string path, string entityName, DataLookupHolder<TV> holder) where T: TV
    {
        holder.Unload();

        if (NeedCancel)
            return;
        
        path = Path.Combine(path, entityName);
        if (!Directory.Exists(path))
            return;

        using var rp1 = Reporter?.ReportGlobal();

        var files = Directory.GetFiles(path, "*.yaml");
        
        Reporter?.ResetTotal(files.Length);
        
        foreach (var file in files)
        {
            if (NeedCancel)
                break;

            if (!int.TryParse(Path.GetFileNameWithoutExtension(file), out var id))
                continue;

            using var rp2 = Reporter?.Report(entityName, id);
            
            holder.Set(id, LoadEntity<T>(file), false);
        }
    }

    protected void LoadHolder<T, TV>(string path, string entityName, DataHolder<TV> holder) where T: TV
    {
        if (NeedCancel)
            return;

        using var rp1 = Reporter?.ReportGlobal();
        
        Reporter?.ResetTotal(1);
        using var rp2 = Reporter?.Report(entityName);
        
        holder.Set(LoadEntity<T>(Path.Combine(path, entityName + ".yaml")));
    }

    public void DumpEntity<T>(string path, string entityName, T entity)
    {
        if (NeedCancel)
            return;
        
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        OnDump(entity);
        
        File.WriteAllText(Path.Combine(path, entityName + ".yaml"), GetSerializer().Serialize(entity));
    }

    public T LoadEntity<T>(string path)
    {
        var entity = GetDeserializer().Deserialize<T>(File.ReadAllText(path));
        OnLoad(entity);

        return entity;
    }

    protected abstract void OnLoad<T>(T entity);
    
    protected abstract void OnDump<T>(T entity);

    private ISerializer? _serializer = null;
    
    private IDeserializer? _deserializer = null;

    private ISerializer GetSerializer()
    {
        return _serializer ??= CreateSerializer();
    }
    
    private IDeserializer GetDeserializer()
    {
        return _deserializer ??= CreateDeserializer();
    }

    protected abstract ISerializer CreateSerializer();
    
    protected abstract IDeserializer CreateDeserializer();
}
