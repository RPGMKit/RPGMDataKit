using System.Diagnostics;
using RPGMakerLibCoreV2.Data;
using RPGMakerLibCoreV2.Data.Helpers.Dump;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages.Abstraction;
using RPGMakerLibCoreV2.Storages.Dump.TextSerializers;

namespace RPGMakerLibCoreV2.Storages.Dump;

public abstract class AbstractTextStorage
{
    private IToTextSerializer _serializer;

    private Dictionary<Type, IToTextSerializer> _specialSerializers = new();

    public AbstractTextStorage(IToTextSerializer serializer)
    {
        _serializer = serializer;
    }

    protected void AddSpecialSerializer<T>(IToTextSerializer serializer)
    {
        _specialSerializers[typeof(T)] = serializer;
    }

    public ProgressReporter? Reporter { get; set; }

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

        using var _ = Reporter?.IncLevel(storage.MapInfos.Get().Count);
        using var _2 = Reporter?.Report(level: _.Level - 1);

        foreach (var (mapId, _) in storage.MapInfos.Get())
        {
            if (NeedCancel)
                break;

            using var _3 = Reporter?.Report($"{nameof(storage.Maps)}: {mapId}");

            var map = storage.Maps.Get(mapId);
            if (map != null)
                DumpEntity(Path.Combine(path, nameof(storage.Maps)), mapId.ToString(), map);
        }
    }

    protected abstract string GetExtension<T>();

    protected void LoadMaps<T>(string path, Storage storage) where T : IMap
    {
        if (NeedCancel)
            return;

        var mapsDirectory = Path.Combine(path, nameof(storage.Maps));
        if (!Directory.Exists(mapsDirectory))
            return;

        var mapFiles = Directory.GetFiles(mapsDirectory, "*." + GetExtension<T>());

        using var _ = Reporter?.IncLevel(mapFiles.Length);
        using var _2 = Reporter?.Report(level: _.Level - 1);

        foreach (var mapFile in mapFiles)
        {
            if (NeedCancel)
                break;

            if (!int.TryParse(Path.GetFileNameWithoutExtension(mapFile), out var id))
                continue;

            using var _3 = Reporter?.Report($"{nameof(storage.Maps)}: {id}");

            var map = LoadEntity<T>(mapFile);

            storage.Maps.Set(id, map);
        }
    }

    protected void DumpHolder<T>(string path, string entityName, DataLookupHolder<T> holder)
    {
        if (NeedCancel)
            return;

        path = Path.Combine(path, entityName);

        using var _ = Reporter?.IncLevel(holder.Get().Count);
        using var _2 = Reporter?.Report(level: _.Level - 1);

        foreach (var (i, e) in holder.Get())
        {
            if (NeedCancel)
                break;

            using var _3 = Reporter?.Report($"{entityName}: {i}");

            if (e != null)
                DumpEntity(path, i.ToString(), e);
        }
    }

    protected void DumpHolder<T>(string path, string entityName, DataHolder<T> holder)
    {
        if (NeedCancel)
            return;

        using var _ = Reporter?.IncLevel(1);
        using var _2 = Reporter?.Report(level: _.Level - 1);
        using var _3 = Reporter?.Report(entityName);
        DumpEntity(path, entityName, holder.Get());
    }

    protected void LoadHolder<T, TV>(string path, string entityName, DataLookupHolder<TV> holder) where T : TV
    {
        holder.Unload();

        if (NeedCancel)
            return;

        path = Path.Combine(path, entityName);
        if (!Directory.Exists(path))
            return;

        var files = Directory.GetFiles(path, "*." + GetExtension<T>());

        using var _ = Reporter?.IncLevel(files.Length);
        using var _2 = Reporter?.Report(level: _.Level - 1);

        foreach (var file in files)
        {
            if (NeedCancel)
                break;

            if (!int.TryParse(Path.GetFileNameWithoutExtension(file), out var id))
                continue;

            using var _3 = Reporter?.Report($"{entityName} {id}");

            holder.Set(id, LoadEntity<T>(file), false);
        }
    }

    protected void LoadHolder<T, TV>(string path, string entityName, DataHolder<TV> holder) where T : TV
    {
        if (NeedCancel)
            return;

        using var _ =Reporter?.IncLevel(1);
        using var _2 = Reporter?.Report(level: _.Level - 1);
        using var _3 = Reporter?.Report(entityName);

        holder.Set(LoadEntity<T>(Path.Combine(path, entityName + "." + GetExtension<T>())));
    }

    private IToTextSerializer GetSerializer<T>() => _specialSerializers.GetValueOrDefault(typeof(T), _serializer);

    public void DumpEntity<T>(string path, string entityName, T entity)
    {
        if (NeedCancel)
            return;

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        File.WriteAllText(Path.Combine(path, entityName + "." + GetExtension<T>()), GetSerializer<T>().Serialize(entity));
    }

    public T LoadEntity<T>(string path)
    {
        Console.WriteLine($"Loading entity at: {path}");
        var entity = GetSerializer<T>().Deserialize<T>(File.ReadAllText(path));

        return (T)entity;
    }
}
