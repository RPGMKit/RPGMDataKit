using System.Collections;
using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public class LookupWrapper<T> : IDataLookup<T?>
{
    private readonly IDataLookup<object> _lookup;

    public LookupWrapper(IDataLookup<object> lookup)
    {
        _lookup = lookup;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _lookup.Cast<T>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool HasIndex(int index) => _lookup.HasIndex(index);

    public T? Get(int index) => (T?)_lookup.Get(index);

    public void Set(int index, T? value) => _lookup.Set(index, value);
}
