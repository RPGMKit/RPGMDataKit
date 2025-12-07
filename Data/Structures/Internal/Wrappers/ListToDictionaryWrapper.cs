using System.Collections;

namespace RPGMakerLibCoreV2.Data.Structures.Internal.Wrappers;

public class ListToDictionaryWrapper<TBaseType, TTargetType> : AbstractWrapper<TBaseType, TTargetType>,
    IDictionary<int, TBaseType?>
{
    private IList<TTargetType?>? _list;

    public static ListToDictionaryWrapper<TBaseType, TTargetType> Create(object source, string memberName,
        Func<TTargetType?, TBaseType?>? getConverter = null,
        Func<TBaseType?, TTargetType?>? setConverter = null)
    {
        return new ListToDictionaryWrapper<TBaseType, TTargetType>(source, memberName, getConverter, setConverter);
    }

    private ListToDictionaryWrapper(object source, string memberName, Func<TTargetType?, TBaseType?>? getConverter,
        Func<TBaseType?, TTargetType?>? setConverter) : base(source, memberName, getConverter, setConverter)
    {
        if (!typeof(IList<TTargetType>).IsAssignableFrom(_member.MemberType))
            throw new Exception("Member is not of type IList<TTargetType>");

        _list = (IList<TTargetType?>?)_member.GetValue(source);
    }

    private IList<TTargetType?> GetList()
    {
        if (_list == null)
        {
            var instance = Activator.CreateInstance(_member.MemberType);
            _member.SetValue(_source, instance);

            _list = instance as IList<TTargetType?>;
        }

        return _list!;
    }

    public IEnumerator<KeyValuePair<int, TBaseType?>> GetEnumerator()
    {
        for (var i = 0; i < _list?.Count; ++i)
            yield return new KeyValuePair<int, TBaseType?>(i, ConvertToBase(_list![i]));
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(KeyValuePair<int, TBaseType?> item) => GetList().Set(item.Key, ConvertToTarget(item.Value));

    public void Clear() => _list?.Clear();

    public bool Contains(KeyValuePair<int, TBaseType?> item)
    {
        if (_list == null)
            return false;

        return _list.Get(item.Key)?.Equals(item.Value) ?? false;
    }

    public void CopyTo(KeyValuePair<int, TBaseType?>[] array, int arrayIndex)
    {
        _list?.Select((t, i) => new KeyValuePair<int, TBaseType?>(i, ConvertToBase(t)))
            .ToArray().CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<int, TBaseType?> item)
    {
        if (_list == null)
            return false;

        if (item.Key < 0 || item.Key >= _list.Count)
            return false;

        var oldValue = _list[item.Key];
        if (oldValue?.Equals(item) ?? false)
        {
            _list[item.Key] = default;
            return true;
        }

        return false;
    }

    public int Count => _list?.Count ?? 0;

    public bool IsReadOnly => _list?.IsReadOnly ?? false;

    public void Add(int key, TBaseType? value) => GetList().Set(key, ConvertToTarget(value));

    public bool ContainsKey(int key) => key >= 0 && key < _list?.Count;

    public bool Remove(int key)
    {
        if (key >= 0 && key < _list?.Count)
        {
            _list[key] = default;
            return true;
        }

        return false;
    }

    public bool TryGetValue(int key, out TBaseType? value)
    {
        if (key >= 0 && key < _list?.Count)
        {
            value = ConvertToBase(_list[key]);
            return true;
        }

        value = default;
        return false;
    }

    public TBaseType? this[int key]
    {
        get => _list != null ? ConvertToBase(_list.Get(key)) : default;
        set => GetList().Set(key, ConvertToTarget(value));
    }

    public ICollection<int> Keys => _list?.Count > 0 ? Enumerable.Range(0, _list.Count).ToArray() : [];

    public ICollection<TBaseType?> Values => _list?.Select(ConvertToBase).ToArray() ?? [];
}
