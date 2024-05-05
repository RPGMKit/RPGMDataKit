using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public class LookupMap<T> : Dictionary<int, T>, IDataLookup<T>
{
    public bool HasIndex(int index) => ContainsKey(index);

    public T Get(int index)
    {
        if (HasIndex(index))
            return this[index];

        return default;
    }

    public void Set(int index, T value)
    {
        this[index] = value;
    }
}
