using RPGMDataKit.Data.Enums.Internal;
using RPGMDataKit.Data.Structures.Interfaces;


namespace RPGMDataKit.Data;

public class DataLookupHolder<TDataType> : DataHolder<Dictionary<int, TDataType?>>
{
    public DataLookupHolder(HolderType type, string path,
        HolderLoader<Dictionary<int, TDataType?>> loader, HolderSaver saver,
        HolderLoaderAsync<Dictionary<int, TDataType?>>? loaderAsync = null, HolderSaverAsync? saverAsync = null)
        : base(path, loader, saver, loaderAsync, saverAsync)
    {
        Type = type;
    }

    private HolderType Type { get; init; }

    internal override Task Accept(IHolderVisitor visitor, string name) => visitor.Visit(this, name);

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
        Set(id, value);

        return id;
    }

    /// <summary>
    /// Puts the entity under the id, replacing the one that is there. The entity gets the id as its <see cref="IHasId.Id"/>.
    /// <c>null</c> makes the slot empty (it is saved as null).
    /// </summary>
    public void Set(int id, TDataType? value)
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

    /// <summary>
    /// Makes the list empty without reading the file (it can be absent): the start of a list that is built from nothing,
    /// for example by a conversion. What was in the file is lost when the list is saved.
    /// </summary>
    public void Clear()
    {
        _data = new();
        _isLoaded = true;
    }

    public override void Save()
    {
        var data = DataToSave();
        if (data != null)
            _Save(data);
    }

    public override Task SaveAsync(CancellationToken cancellationToken = default)
    {
        var data = DataToSave();

        return data != null ? _SaveAsync(data, cancellationToken) : Task.CompletedTask;
    }

    /// <summary>What goes to the file: the dictionary as it is, a list made of the dictionary by the ids; null when there is nothing to save.</summary>
    private object? DataToSave()
    {
        if (Type == HolderType.Dictionary)
            return _data;

        if (_data == default)
            return null;

        var list = new TDataType?[_data.Count == 0 ? 0 : _data.Max(p => p.Key + 1)];
        foreach (var (k, v) in _data)
            list[k] = v;

        return list;
    }
}
