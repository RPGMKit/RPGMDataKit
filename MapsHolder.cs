using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2;

public class MapsHolder
{
    private readonly Dictionary<int, DataHolder<IMap>> _maps = new();
    private readonly Storage _storage;

    public MapsHolder(Storage storage)
    {
        _storage = storage;
    }

    private string MapPath(int index) => _storage.DataPath($"Map{index:D3}.json");

    public IMap? Get(int index)
    {
        if (!_maps.ContainsKey(index))
        {
            try
            {
                var h = CreateSubHolder(index);
                h.Load();
                _maps[index] = h;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        return _maps[index].Get();
    }

    private DataHolder<IMap> CreateSubHolder(int mapIndex)
    {
        var path = MapPath(mapIndex);
        DataHolder<IMap> holder = new(_storage, () => new()
        {
            Path = path,
            Data = _storage.LoadData<Map>(path)
        });

        return holder;
    }

    public void Set(int index, Map data)
    {
        var h = CreateSubHolder(index);
        h.Set(data);

        _maps[index] = h;
    }

    public void Save()
    {
        foreach (var (_, map) in _maps)
            map.Save();
    }
}
