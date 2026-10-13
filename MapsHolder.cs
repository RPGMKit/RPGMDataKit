using RPGMDataKit.Data;
using RPGMDataKit.Data.Structures.Interfaces;


namespace RPGMDataKit;

public class MapsHolder : ITable
{
    private readonly Dictionary<int, DataHolder<IMap>> _maps = new();
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

    /// <summary>Returns the map, loading it on first access, or null when the map file does not exist.</summary>
    public IMap? Get(int index)
    {
        if (_maps.TryGetValue(index, out var holder))
            return holder.Get();

        holder = CreateSubHolder(index);
        holder.Load();

        var map = holder.Get();
        if (map != null)
            _maps[index] = holder;

        return map;
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
        foreach (var holder in _maps.Values)
            holder.Save();
    }

    /// <summary>Saves one map; does nothing when the map was not loaded.</summary>
    public void Save(int index)
    {
        if (_maps.TryGetValue(index, out var holder))
            holder.Save();
    }
}
