using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public class LookupList<T> : List<T?>, IDataLookup<T?>
{
    public LookupList() { }
    
    public LookupList(IList<T?> list) : base(list) { }
    
    public bool HasIndex(int index) => index < Count;
    
    public T? Get(int index)
    {
        if (HasIndex(index))
            return this[index];

        return default;
    }

    public void Set(int index, T? value)
    {
        while (index >= Count)
            Add(default);

        this[index] = value;
    }
}
