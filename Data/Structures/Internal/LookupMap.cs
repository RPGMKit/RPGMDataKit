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

    public void Set(int index, T? value)
    {
        this[index] = value;
    }

    public IEnumerator<T> GetEnumerator() => Values.GetEnumerator();

    public IEnumerable<KeyValuePair<int, T>> GetWithIndex()
    {
        foreach (var key in Keys)
            yield return new KeyValuePair<int, T>(key, this[key]);
    }
    
    public IDataLookup<TV> MorphToBase<TV>() where TV : class
    {
        if (!typeof(TV).IsSubclassOf(typeof(T)))
            throw new Exception("Cant morph, not a base type.");

        LookupMap<TV> morphed = new LookupMap<TV>();
        foreach (var p in GetWithIndex())
        {
            morphed.Set(p.Key, (TV?)Convert.ChangeType(p.Value, typeof(TV)));
        }

        return morphed;
    }
}
