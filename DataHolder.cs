using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2;

public class DataHolder<TDataType>
{
    private bool _isLoaded;
    private readonly Storage _storage;
    private TDataType _data;
    private readonly Func<HolderInfo> _loader;
    private string _path;
    
    public class HolderInfo
    {
        public TDataType Data;
        public string Path;
    }
    
    public DataHolder(Storage storage, Func<HolderInfo> loader)
    {
        _storage = storage;
        _loader = loader;
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
        var h = _loader();
        _data = h.Data;
        _path = h.Path;
        _isLoaded = true;
    }

    public void Set(TDataType value)
    {
        _data = value;
        _isLoaded = true;
    }

    public void Save()
    {
        if (!_isLoaded)
            return;
        
        _storage.SaveData(_path, _data);
    }
}
