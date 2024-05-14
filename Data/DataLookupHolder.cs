using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data;

public class DataLookupHolder<TDataType> : DataHolder<IByIdGetter<TDataType>>
{
    public DataLookupHolder(Storage storage, string path, HolderLoader<IByIdGetter<TDataType>> loader) : base(storage, path, loader)
    {
    }

    public TDataType Get(int index)
    {
        return Get().Get(index);
    }
}
