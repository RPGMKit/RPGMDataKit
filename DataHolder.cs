namespace RPGMakerLibCoreV2;

public class DataHolder<T>
{
    private bool _isLoaded;
    private readonly Storage _storage;
    private T _data;
    private readonly string _path;

    public DataHolder(Storage storage, string path)
    {
        _storage = storage;
        _path = path;
    }

    public T Get()
    {
        if (!_isLoaded)
        {
            _data = _storage.LoadData<T>(_path);
            _isLoaded = true;
        }

        return _data;
    }

    public void Set(T value)
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
