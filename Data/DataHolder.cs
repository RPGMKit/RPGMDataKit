using RPGMDataKit.Storages.Abstraction;

namespace RPGMDataKit.Data;

public delegate T? HolderLoader<T>(string path);

public delegate void HolderSaver(string path, object? data);

public delegate Task<T?> HolderLoaderAsync<T>(string path, CancellationToken cancellationToken);

public delegate Task HolderSaverAsync(string path, object? data, CancellationToken cancellationToken);

public class DataHolder<TDataType> : IHolder
{
    protected bool _isLoaded;
    private readonly HolderSaver _saver;
    private readonly HolderSaverAsync? _saverAsync;
    protected TDataType _data;
    private readonly HolderLoader<TDataType> _loader;
    private readonly HolderLoaderAsync<TDataType>? _loaderAsync;
    private string _path;

    /// <param name="loaderAsync">Reads the data without blocking; without it <see cref="LoadAsync"/> runs <paramref name="loader"/> on the calling thread.</param>
    /// <param name="saverAsync">Writes the data without blocking; without it <see cref="SaveAsync"/> runs <paramref name="saver"/> on the calling thread.</param>
    public DataHolder(string path, HolderLoader<TDataType> loader, HolderSaver saver,
        HolderLoaderAsync<TDataType>? loaderAsync = null, HolderSaverAsync? saverAsync = null)
    {
        _saver = saver;
        _saverAsync = saverAsync;
        _path = path;
        _loader = loader;
        _loaderAsync = loaderAsync;
    }

    public void Unload()
    {
        _isLoaded = false;
        _data = default;
    }
    
    public TDataType Get()
    {
        if (!_isLoaded)
        {
            Load();

            _isLoaded = true;
        }

        return _data;
    }

    void IHolder.Preload() => Get();

    Task IHolder.PreloadAsync(CancellationToken cancellationToken) => LoadAsync(cancellationToken);

    Task IHolder.Accept(IHolderVisitor visitor, string name) => Accept(visitor, name);

    internal virtual Task Accept(IHolderVisitor visitor, string name) => visitor.Visit(this, name);

    public void Load()
    {
        if (_isLoaded)
            return;
        
        _data = _loader(_path);
        _isLoaded = true;
    }

    /// <summary>Like <see cref="Load"/>, without blocking the thread while the file is read.</summary>
    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (_isLoaded)
            return;

        _data = _loaderAsync != null ? await _loaderAsync(_path, cancellationToken) : _loader(_path);
        _isLoaded = true;
    }

    /// <summary>Like <see cref="Get()"/>, loading the data without blocking the thread.</summary>
    public async Task<TDataType> GetAsync(CancellationToken cancellationToken = default)
    {
        await LoadAsync(cancellationToken);

        return _data;
    }

    public void Set(TDataType value)
    {
        _data = value;
        _isLoaded = true;
    }

    public virtual void Save()
    {
        _Save(_data);
    }

    /// <summary>Like <see cref="Save"/>, without blocking the thread while the file is written.</summary>
    public virtual Task SaveAsync(CancellationToken cancellationToken = default)
    {
        return _SaveAsync(_data, cancellationToken);
    }

    protected void _Save<T>(T value)
    {
        if (!_isLoaded)
            return;

        _saver(_path, value);
    }

    protected Task _SaveAsync<T>(T value, CancellationToken cancellationToken)
    {
        if (!_isLoaded)
            return Task.CompletedTask;

        if (_saverAsync != null)
            return _saverAsync(_path, value, cancellationToken);

        _saver(_path, value);

        return Task.CompletedTask;
    }
}
