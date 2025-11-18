using RPGMakerLibCoreV2.Data.Enums.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data;

public class DataLookupHolder<TDataType> : DataHolder<Dictionary<int, TDataType?>>
{
    public DataLookupHolder(Storage storage, HolderType type, string path,
        HolderLoader<Dictionary<int, TDataType?>> loader) : base(storage, path, loader)
    {
        Type = type;
    }

    private HolderType Type { get; init; }

    public TDataType? Get(int index)
    {
        return Get().GetValueOrDefault(index);
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
