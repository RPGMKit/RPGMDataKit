using System.Collections;
using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public class ListWrapper<T, TV> : IDataLookup<T>
{
    private readonly IList<TV?> _list;
    private readonly Func<T?, TV?> _getter;
    private readonly Func<TV?, T?> _setter;

    public ListWrapper(IList<TV?> list, Func<T?, TV?> getter, Func<TV?, T?> setter)
    {
        _list = list;
        _getter = getter;
        _setter = setter;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _list.Select(i => _setter(i)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool HasIndex(int index) => index < _list.Count;

    public T? Get(int index) => HasIndex(index) ? _setter(_list[index]) : default;

    public void Set(int index, T? value)
    {
        _list.Grow(index + 1);

        _list[index] = _getter(value);
    }
}
