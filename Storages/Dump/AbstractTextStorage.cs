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
    private readonly Storage _storage;

    private IToTextSerializer _serializer;

    private Dictionary<Type, IToTextSerializer> _specialSerializers = new();

    /// <summary>Made by <see cref="Storage.CreateYamlStorage"/>, for the storage it is made by.</summary>
    internal AbstractTextStorage(Storage storage, IToTextSerializer serializer)
    {
        _storage = storage;
        _serializer = serializer;
    }

    protected void AddSpecialSerializer<T>(IToTextSerializer serializer)
    {
        _specialSerializers[typeof(T)] = serializer;
    }

    public ProgressReporter? Reporter { get; set; }

    private bool NeedCancel => Reporter?.NeedCancel ?? false;

    /// <summary>Writes the storage into the folder (it must exist).</summary>
    public async Task Dump(string path)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist");

        await _Dump(path, _storage);
    }

    /// <summary>Dumps every holder of the storage (the standard ones and the ones added to it), the maps last.</summary>
    protected virtual async Task _Dump(string path, Storage storage)
    {
        Reporter?.SetStage(DumpStage.Progress);
        Reporter?.ResetTotal(storage.Holders.Count);

        var visitor = new DumpVisitor(this, path);
        foreach (var (name, holder) in storage.Holders)
            await holder.Accept(visitor, name);

        await DumpMaps(path, storage);

        Reporter?.SetStage(DumpStage.Finished);
    }

    /// <summary>Fills the storage from a dump in the folder; a holder that the dump does not have is left as it is.</summary>
    public void Load(string path)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist");

        _Load(path, _storage);
    }

    /// <summary>Fills every holder of the storage from the dump; what the dump does not have is left as it is.</summary>
    protected virtual void _Load(string path, Storage storage)
    {
        Reporter?.SetStage(DumpStage.Progress);
        Reporter?.ResetTotal(storage.Holders.Count);

        var visitor = new LoadVisitor(this, storage, path);
        foreach (var (name, holder) in storage.Holders)
            holder.Accept(visitor, name);

        LoadMaps(path, storage);

        Reporter?.SetStage(DumpStage.Finished);
    }

    /// <summary>Like <see cref="Load"/>, without blocking the thread while the files of the dump are read.</summary>
    public async Task LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(path))
            throw new Exception($"Directory {path} does not exist");

        await _LoadAsync(path, _storage, cancellationToken);
    }

    /// <summary>Like <see cref="_Load"/>; the holders are filled one after another (the text is parsed one file at a time).</summary>
    protected virtual async Task _LoadAsync(string path, Storage storage, CancellationToken cancellationToken)
    {
        Reporter?.SetStage(DumpStage.Progress);
        Reporter?.ResetTotal(storage.Holders.Count);

        var visitor = new LoadAsyncVisitor(this, storage, path, cancellationToken);
        foreach (var (name, holder) in storage.Holders)
            await holder.Accept(visitor, name);

        await LoadMapsAsync(path, storage, cancellationToken);

        Reporter?.SetStage(DumpStage.Finished);
    }

    private sealed class LoadAsyncVisitor(AbstractTextStorage owner, Storage storage, string path, CancellationToken token) : IHolderVisitor
    {
        public Task Visit<T>(DataLookupHolder<T> holder, string name) =>
            owner.InvokeCoreAsync(nameof(LoadHolderCoreAsync), [storage.ConcreteType<T>(), typeof(T)], path, name, holder, token);

        public Task Visit<T>(DataHolder<T> holder, string name) =>
            owner.InvokeCoreAsync(nameof(LoadSingleHolderCoreAsync), [storage.ConcreteType<T>(), typeof(T)], path, name, holder, token);

        public Task Visit(MapsHolder holder, string name) => Task.CompletedTask;
    }

    // the maps are not a holder like the others: a file for each map, dumped and loaded after the rest (DumpMaps, LoadMaps)
    private sealed class DumpVisitor(AbstractTextStorage owner, string path) : IHolderVisitor
    {
        public Task Visit<T>(DataLookupHolder<T> holder, string name) => owner.DumpHolder(path, name, holder);

        public Task Visit<T>(DataHolder<T> holder, string name) => owner.DumpHolder(path, name, holder);

        public Task Visit(MapsHolder holder, string name) => Task.CompletedTask;
    }

    private sealed class LoadVisitor(AbstractTextStorage owner, Storage storage, string path) : IHolderVisitor
    {
        public Task Visit<T>(DataLookupHolder<T> holder, string name)
        {
            owner.LoadHolder(storage, path, name, holder);

            return Task.CompletedTask;
        }

        public Task Visit<T>(DataHolder<T> holder, string name)
        {
            owner.LoadHolder(storage, path, name, holder);

            return Task.CompletedTask;
        }

        public Task Visit(MapsHolder holder, string name) => Task.CompletedTask;
    }

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

    private Task InvokeCoreAsync(string method, Type[] typeArgs, params object?[] args)
    {
        return (Task)typeof(AbstractTextStorage)
            .GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!
            .MakeGenericMethod(typeArgs)
            .Invoke(this, args)!;
    }

    protected void LoadMaps(string path, Storage storage)
    {
        InvokeCore(nameof(LoadMapsCore), [storage.ConcreteType<IMap>()], path, storage);
    }

    protected Task LoadMapsAsync(string path, Storage storage, CancellationToken cancellationToken)
    {
        return InvokeCoreAsync(nameof(LoadMapsCoreAsync), [storage.ConcreteType<IMap>()], path, storage, cancellationToken);
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

    private async Task LoadMapsCoreAsync<T>(string path, Storage storage, CancellationToken token) where T : IMap
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

            var map = await LoadEntityAsync<T>(mapFile, token);

            storage.Maps.Set(id, map);
        }
    }

    private async Task LoadHolderCoreAsync<T, TV>(string path, string entityName, DataLookupHolder<TV> holder, CancellationToken token)
        where T : TV
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

            holder.Set(id, await LoadEntityAsync<T>(file, token));
        }
    }

    private async Task LoadSingleHolderCoreAsync<T, TV>(string path, string entityName, DataHolder<TV> holder, CancellationToken token)
        where T : TV
    {
        if (NeedCancel)
            return;

        using var _ = Reporter?.IncLevel(1);
        using var _2 = Reporter?.DoReport(level: _.Level - 1);
        using var _3 = Reporter?.DoReport(entityName);

        var file = Path.Combine(path, entityName + "." + GetExtension<T>());
        if (File.Exists(file))
            holder.Set(await LoadEntityAsync<T>(file, token));
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

        // a dump made before the holder was added does not have it: the holder is left as it is, as a list is
        var file = Path.Combine(path, entityName + "." + GetExtension<T>());
        if (File.Exists(file))
            holder.Set(LoadEntity<T>(file));
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
        var entity = GetSerializer<T>().Deserialize<T>(File.ReadAllText(path));

        return (T)entity;
    }

    /// <summary>Like <see cref="LoadEntity{T}"/>, without blocking the thread while the file is read.</summary>
    public async Task<T> LoadEntityAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        var entity = GetSerializer<T>().Deserialize<T>(await File.ReadAllTextAsync(path, cancellationToken));

        return (T)entity;
    }
}
