using RPGMDataKit.Storages.Abstraction;

namespace RPGMDataKit.Data;

public delegate T? HolderLoader<T>(string path);

public delegate void HolderSaver(string path, object? data);

public class DataHolder<TDataType> : IHolder
{
    protected bool _isLoaded;
    private readonly HolderSaver _saver;
    protected TDataType _data;
    private readonly HolderLoader<TDataType> _loader;
    private string _path;

    public DataHolder(string path, HolderLoader<TDataType> loader, HolderSaver saver)
    {
        _saver = saver;
        _path = path;
        _loader = loader;
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

    Task IHolder.Accept(IHolderVisitor visitor, string name) => Accept(visitor, name);

    internal virtual Task Accept(IHolderVisitor visitor, string name) => visitor.Visit(this, name);

    public void Load()
    {
        if (_isLoaded)
            return;
        
        _data = _loader(_path);
        _isLoaded = true;
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

    protected void _Save<T>(T value)
    {
        if (!_isLoaded)
            return;

        _saver(_path, value);
    }
}
