using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public class LookupList<T> : List<T>, IDataLookup<T>
{
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

    public IEnumerable<KeyValuePair<int, T>> GetWithIndex()
    {
        for (var i = 0; i < Count; ++i)
            yield return new KeyValuePair<int, T>(i, this[i]);
    }

    public IDataLookup<TV> MorphToBase<TV>() where TV : class
    {
        if (!typeof(TV).IsAssignableFrom(typeof(T)))
            throw new Exception("Cant morph, not a base type.");

        LookupList<TV> morphed = new LookupList<TV>();
        foreach (var p in GetWithIndex())
        {
            morphed.Set(p.Key, p.Value as TV);
        }

        return morphed;
    }
}