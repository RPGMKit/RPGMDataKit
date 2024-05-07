using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2;

public class DataHolder<TDataType>
{
    private bool _isLoaded;
    private readonly Storage _storage;
    private TDataType _data;
    private readonly Func<string, TDataType> _loader;
    private string _path;

    public DataHolder(Storage storage, string path, Func<string, TDataType> loader)
    {
        _storage = storage;
        _path = path;
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
        _data = _loader(_path);
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
