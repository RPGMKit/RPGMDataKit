using RPGMakerLibCoreV2.Data.Enums.Internal;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;


namespace RPGMakerLibCoreV2.Data;

public class DataLookupHolder<TDataType> : DataHolder<Dictionary<int, TDataType?>>
{
    public DataLookupHolder(HolderType type, string path,
        HolderLoader<Dictionary<int, TDataType?>> loader, HolderSaver saver) : base(path, loader, saver)
    {
        Type = type;
    }

    private HolderType Type { get; init; }

    public TDataType? Get(int index)
    {
        return Get().GetValueOrDefault(index);
    }

    /// <summary>Id for a new entity: one past the highest used, starting from 1 (slot 0 is reserved).</summary>
    public int NextId()
    {
        var data = Get();
        return data.Count == 0 ? 1 : Math.Max(data.Keys.Max() + 1, 1);
    }

    /// <summary>Appends the entity under <see cref="NextId"/> and returns that id.</summary>
    public int Add(TDataType value)
    {
        var id = NextId();
        Replace(id, value);

        return id;
    }

    /// <summary>Puts the entity under the given id, replacing any existing one.</summary>
    public void Replace(int id, TDataType value)
    {
        if (value is IHasId entity)
            entity.Id = id;

        Get()[id] = value;
    }

    /// <summary>Removes the entity. Other ids are not shifted; a removed slot is saved as null.</summary>
    public bool Remove(int id)
    {
        return Get().Remove(id);
    }

    public void Set(int index, TDataType? value, bool load = true)
    {
        if (load)
        {
            Get()[index] = value;
            return;
        }

        if (!_isLoaded)
        {
            _data = new();
            _isLoaded = true;
        }

        _data[index] = value;
    }

    public override void Save()
    {
        if (Type == HolderType.Dictionary)
        {
            _Save(_data);
            return;
        }

        if (_data == default)
            return;

        var list = new TDataType?[_data.Max(p => p.Key + 1)];
        foreach (var (k, v) in _data)
            list[k] = v;

        _Save(list);
    }
}
