using RPGMakerLibCoreV2.Data;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;


namespace RPGMakerLibCoreV2;

public class MapsHolder : ITable
{
    private readonly Dictionary<int, DataHolder<IMap>?> _maps = new();
    private readonly HolderSaver _saver;
    private readonly Func<int, string> _pathLoader;
    private readonly HolderLoader<IMap> _loader;

    public MapsHolder(Func<int, string> pathLoader, HolderLoader<IMap> loader, HolderSaver saver)
    {
        _saver = saver;
        _pathLoader = pathLoader;
        _loader = loader;
    }

    void ITable.Preload()
    {
    }

    public void Unload()
    {
        _maps.Clear();
    }

    public IMap? Get(int index)
    {
        if (_maps.TryGetValue(index, out var map))
            return map?.Get();

        try
        {
            map = CreateSubHolder(index);
            map.Load();
            _maps[index] = map;

            return map.Get();
        }
        catch (FileNotFoundException)
        {
            _maps[index] = null;
            return null;
        }
    }

    private DataHolder<IMap> CreateSubHolder(int mapIndex)
    {
        return new(_pathLoader(mapIndex), _loader, _saver);
    }

    public void Set(int index, IMap data)
    {
        var h = CreateSubHolder(index);
        h.Set(data);

        _maps[index] = h;
    }

    public void Save()
    {
        foreach (var (_, map) in _maps)
            map?.Save();
    }
}
