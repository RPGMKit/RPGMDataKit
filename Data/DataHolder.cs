using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data;

public delegate T? HolderLoader<T>(string path);

public class DataHolder<TDataType>
{
    protected bool _isLoaded;
    private readonly Storage _storage;
    protected TDataType _data;
    private readonly HolderLoader<TDataType> _loader;
    private string _path;

    public DataHolder(Storage storage, string path, HolderLoader<TDataType> loader)
    {
        _storage = storage;
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

        _storage.SaveData(_path, value);
    }
}
