using RPGMakerLibCoreV2.Data;
using RPGMakerLibCoreV2.Storages.Abstraction;
using YamlDotNet.Serialization;

namespace RPGMakerLibCoreV2.Storages.Dump;

public abstract class AbstractYamlDumper
{
    public void Dump(string path, Storage storage)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist");

        _Dump(path, storage);
    }

    protected abstract void _Dump(string path, Storage storage);

    public void Load(string path, Storage storage)
    {
        _Load(path, storage);
    }

    protected abstract void _Load(string path, Storage storage);
    
    protected void DumpMaps(string path, Storage storage)
    {
        foreach (var (mapId, _) in storage.MapInfos.Get().WithIndex())
        {
            var map = storage.Maps.Get(mapId);
            if (map != null)
                DumpEntity(Path.Combine(path, nameof(storage.Maps)), $"{mapId:D3}", map);
        }
    }

    protected void DumpEntity<T>(string path, string entityName, DataLookupHolder<T> entity)
    {
        path = Path.Combine(path, entityName);

        foreach (var (i, e) in entity.Get().WithIndex())
            if (e != null)
                DumpEntity(path, $"{i:D3}", e);
    }

    protected void DumpEntity<T>(string path, string entityName, DataHolder<T> entity)
    {
        DumpEntity(path, entityName, entity.Get());
    }

    public void DumpEntity<T>(string path, string entityName, T entity)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        File.WriteAllText(Path.Combine(path, entityName + ".yaml"), GetSerializer().Serialize(entity));
    }

    public abstract ISerializer GetSerializer();
    
    public abstract IDeserializer GetDeserializer();
}
