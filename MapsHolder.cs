using RPGMakerLibCoreV2.Data.Structures;

namespace RPGMakerLibCoreV2;

public class MapsHolder
{
    private Dictionary<int, DataHolder<Map>> _maps = new();
    private readonly Storage _storage;

    public MapsHolder(Storage storage)
    {
        _storage = storage;
    }

    private string MapPath(int index) => _storage.DataPath($"Map{index:D3}.json");

    public Map Get(int index)
    {
        if (!_maps.ContainsKey(index))
        {
            try
            {
                var path = MapPath(index);
                var data = _storage.LoadData<Map>(path);
                _maps[index] = new(_storage, path);
                _maps[index].Set(data);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        return _maps[index].Get();
    }

    public void Set(int index, Map data)
    {
        _maps[index] = new(_storage, MapPath(index));
        _maps[index].Set(data);
    }

    public void Save()
    {
        foreach (var (_, map) in _maps)
            map.Save();
    }
}
