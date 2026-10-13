using System.Diagnostics;
using System.Reflection;
using RPGMDataKit.Data;
using RPGMDataKit.Data.Helpers.Dump;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Storages.Abstraction;
using RPGMDataKit.Storages.Dump.TextSerializers;

namespace RPGMDataKit.Storages.Dump;

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

    public async Task Dump(string path, Storage storage)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist");

        await _Dump(path, storage);
    }

    protected abstract Task _Dump(string path, Storage storage);

    public void Load(string path, Storage storage)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist");

        _Load(path, storage);
    }

    protected abstract void _Load(string path, Storage storage);

    protected async Task DumpMaps(string path, Storage storage)
    {
        if (NeedCancel)
            return;

        using var _ = Reporter?.IncLevel(storage.MapInfos.Get().Count);
        using var _2 = Reporter?.DoReport(level: _.Level - 1);

        foreach (var (mapId, _) in storage.MapInfos.Get())
        {
            if (NeedCancel)
                break;

            using var _3 = Reporter?.DoReport($"{nameof(storage.Maps)}: {mapId}");

            var map = storage.Maps.Get(mapId);
            if (map != null)
                await DumpEntity(Path.Combine(path, nameof(storage.Maps)), mapId.ToString(), map);
        }
    }

    protected abstract string GetExtension<T>();

    private void InvokeCore(string method, Type[] typeArgs, params object?[] args)
    {
        typeof(AbstractTextStorage)
            .GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!
            .MakeGenericMethod(typeArgs)
            .Invoke(this, args);
    }

    protected void LoadMaps(string path, Storage storage)
    {
        InvokeCore(nameof(LoadMapsCore), [storage.ConcreteType<IMap>()], path, storage);
    }

    protected void LoadHolder<TV>(Storage storage, string path, string entityName, DataLookupHolder<TV> holder)
    {
        InvokeCore(nameof(LoadHolderCore), [storage.ConcreteType<TV>(), typeof(TV)], path, entityName, holder);
    }

    protected void LoadHolder<TV>(Storage storage, string path, string entityName, DataHolder<TV> holder)
    {
        InvokeCore(nameof(LoadSingleHolderCore), [storage.ConcreteType<TV>(), typeof(TV)], path, entityName, holder);
    }

    private void LoadMapsCore<T>(string path, Storage storage) where T : IMap
    {
        if (NeedCancel)
            return;

        var mapsDirectory = Path.Combine(path, nameof(storage.Maps));
        if (!Directory.Exists(mapsDirectory))
            return;

        var mapFiles = Directory.GetFiles(mapsDirectory, "*." + GetExtension<T>());

        using var _ = Reporter?.IncLevel(mapFiles.Length);
        using var _2 = Reporter?.DoReport(level: _.Level - 1);

        foreach (var mapFile in mapFiles)
        {
            if (NeedCancel)
                break;

            if (!int.TryParse(Path.GetFileNameWithoutExtension(mapFile), out var id))
                continue;

            using var _3 = Reporter?.DoReport($"{nameof(storage.Maps)}: {id}");

            var map = LoadEntity<T>(mapFile);

            storage.Maps.Set(id, map);
        }
    }

    protected async Task DumpHolder<T>(string path, string entityName, DataLookupHolder<T> holder)
    {
        if (NeedCancel)
            return;

        path = Path.Combine(path, entityName);

        using var _ = Reporter?.IncLevel(holder.Get().Count);
        using var _2 = Reporter?.DoReport(level: _.Level - 1);

        foreach (var (i, e) in holder.Get())
        {
            if (NeedCancel)
                break;

            using var _3 = Reporter?.DoReport($"{entityName}: {i}");

            if (e != null)
                await DumpEntity(path, i.ToString(), e);
        }
    }

    protected async Task DumpHolder<T>(string path, string entityName, DataHolder<T> holder)
    {
        if (NeedCancel)
            return;

        using var _ = Reporter?.IncLevel(1);
        using var _2 = Reporter?.DoReport(level: _.Level - 1);
        using var _3 = Reporter?.DoReport(entityName);
        await DumpEntity(path, entityName, holder.Get());
    }

    private void LoadHolderCore<T, TV>(string path, string entityName, DataLookupHolder<TV> holder) where T : TV
    {
        holder.Unload();

        if (NeedCancel)
            return;

        path = Path.Combine(path, entityName);
        if (!Directory.Exists(path))
            return;

        var files = Directory.GetFiles(path, "*." + GetExtension<T>());

        using var _ = Reporter?.IncLevel(files.Length);
        using var _2 = Reporter?.DoReport(level: _.Level - 1);

        var started = false;
        foreach (var file in files)
        {
            if (NeedCancel)
                break;

            if (!int.TryParse(Path.GetFileNameWithoutExtension(file), out var id))
                continue;

            using var _3 = Reporter?.DoReport($"{entityName} {id}");

            // the list is made of the files of the dump: the data file of the storage is not read
            if (!started)
            {
                holder.Clear();
                started = true;
            }

            holder.Set(id, LoadEntity<T>(file));
        }
    }

    private void LoadSingleHolderCore<T, TV>(string path, string entityName, DataHolder<TV> holder) where T : TV
    {
        if (NeedCancel)
            return;

        using var _ = Reporter?.IncLevel(1);
        using var _2 = Reporter?.DoReport(level: _.Level - 1);
        using var _3 = Reporter?.DoReport(entityName);

        holder.Set(LoadEntity<T>(Path.Combine(path, entityName + "." + GetExtension<T>())));
    }

    private IToTextSerializer GetSerializer<T>() => _specialSerializers.GetValueOrDefault(typeof(T), _serializer);

    public async Task DumpEntity<T>(string path, string entityName, T entity)
    {
        if (NeedCancel)
            return;

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var entityPath = Path.Combine(path, entityName + "." + GetExtension<T>());

        await File.WriteAllTextAsync(entityPath, GetSerializer<T>().Serialize(entity));

        Reporter?.CustomReport(entityPath);
    }

    public T LoadEntity<T>(string path)
    {
        Console.WriteLine($"Loading entity at: {path}");
        var entity = GetSerializer<T>().Deserialize<T>(File.ReadAllText(path));

        return (T)entity;
    }
}
