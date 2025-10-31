using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data;

public class DataLookupHolder<TDataType> : DataHolder<IDataLookup<TDataType>>
{
    public DataLookupHolder(Storage storage, string path, HolderLoader<IDataLookup<TDataType>> loader) : base(storage, path, loader)
    {
    }

    public TDataType Get(int index)
    {
        return Get().Get(index);
    }

    public void Set(int index, TDataType? value)
    {
        Get().Set(index, value);
    }
}
