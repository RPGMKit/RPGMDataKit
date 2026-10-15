using RPGMDataKit.Data;
using RPGMDataKit.Data.Structures.Interfaces;


namespace RPGMDataKit;

public class MapsHolder : IHolder
{
    // the maps are touched by several tasks when they are loaded or saved together (Storage.PreloadAsync, Storage.SaveAllAsync)
    private readonly Dictionary<int, DataHolder<IMap>> _maps = new();
    private readonly HolderSaver _saver;
    private readonly HolderSaverAsync? _saverAsync;
    private readonly Func<int, string> _pathLoader;
    private readonly HolderLoader<IMap> _loader;
    private readonly HolderLoaderAsync<IMap>? _loaderAsync;

    public MapsHolder(Func<int, string> pathLoader, HolderLoader<IMap> loader, HolderSaver saver,
        HolderLoaderAsync<IMap>? loaderAsync = null, HolderSaverAsync? saverAsync = null)
    {
        _saver = saver;
        _saverAsync = saverAsync;
        _pathLoader = pathLoader;
        _loader = loader;
        _loaderAsync = loaderAsync;
    }

    void IHolder.Preload()
    {
    }

    Task IHolder.PreloadAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    Task IHolder.Accept(IHolderVisitor visitor, string name) => visitor.Visit(this, name);

    public void Unload()
    {
        lock (_maps)
            _maps.Clear();
    }

    /// <summary>Returns the map, loading it on first access, or null when the map file does not exist.</summary>
    public IMap? Get(int index)
    {
        lock (_maps)
        {
            if (_maps.TryGetValue(index, out var loaded))
                return loaded.Get();
        }

        var holder = CreateSubHolder(index);
        holder.Load();

        var map = holder.Get();

        return map == null ? null : Remember(index, holder);
    }

    /// <summary>Like <see cref="Get"/>, without blocking the thread while the file is read.</summary>
    public async Task<IMap?> GetAsync(int index, CancellationToken cancellationToken = default)
    {
        DataHolder<IMap>? loaded;
        lock (_maps)
            _maps.TryGetValue(index, out loaded);

        if (loaded != null)
            return loaded.Get();

        var holder = CreateSubHolder(index);
        await holder.LoadAsync(cancellationToken);

        var map = holder.Get();

        return map == null ? null : Remember(index, holder);
    }

    // another task may have loaded the same map meanwhile: the first one stays
    private IMap Remember(int index, DataHolder<IMap> holder)
    {
        lock (_maps)
        {
            if (!_maps.TryGetValue(index, out var first))
                _maps[index] = first = holder;

            return first.Get();
        }
    }

    private DataHolder<IMap> CreateSubHolder(int mapIndex)
    {
        return new(_pathLoader(mapIndex), _loader, _saver, _loaderAsync, _saverAsync);
    }

    public void Set(int index, IMap data)
    {
        var h = CreateSubHolder(index);
        h.Set(data);

        lock (_maps)
            _maps[index] = h;
    }

    public void Save()
    {
        foreach (var holder in Loaded())
            holder.Save();
    }

    /// <summary>Like <see cref="Save()"/>, without blocking the thread while the files are written; the maps are saved at the same time.</summary>
    public Task SaveAsync(CancellationToken cancellationToken = default) => SaveAsync(cancellationToken, 0);

    /// <param name="maxDegreeOfParallelism">How many maps are written at the same time; 0 is the number of processors.</param>
    public Task SaveAsync(CancellationToken cancellationToken, int maxDegreeOfParallelism)
    {
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism > 0 ? maxDegreeOfParallelism : Environment.ProcessorCount,
            CancellationToken = cancellationToken,
        };

        return Parallel.ForEachAsync(Loaded(), options, async (holder, token) => await holder.SaveAsync(token));
    }

    Task IHolder.SaveAsync(CancellationToken cancellationToken) => SaveAsync(cancellationToken);

    /// <summary>Saves one map; does nothing when the map was not loaded.</summary>
    public void Save(int index)
    {
        DataHolder<IMap>? holder;
        lock (_maps)
            _maps.TryGetValue(index, out holder);

        holder?.Save();
    }

    private List<DataHolder<IMap>> Loaded()
    {
        lock (_maps)
            return _maps.Values.ToList();
    }
}
