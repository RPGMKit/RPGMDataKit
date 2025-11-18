using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public class LookupMap<T> : Dictionary<int, T?>, IDataLookup<T?>
{
    public bool HasIndex(int index) => ContainsKey(index);

    public T? Get(int index)
    {
        return HasIndex(index) ? this[index] : default;
    }

    public void Set(int index, T? value)
    {
        this[index] = value;
    }

    public IEnumerator<T> GetEnumerator() => Values.GetEnumerator();
}
