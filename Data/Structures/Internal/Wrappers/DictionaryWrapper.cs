using System.Collections;

namespace RPGMakerLibCoreV2.Data.Structures.Internal.Wrappers;

public class DictionaryWrapper<TBaseType, TTargetType> : AbstractWrapper<TBaseType, TTargetType>,
    IDictionary<int, TBaseType?>
{
    private IDictionary<int, TTargetType?>? _dict;

    public static DictionaryWrapper<TBaseType, TTargetType> Create(object source, string memberName,
        Func<TTargetType?, TBaseType?>? getConverter = null,
        Func<TBaseType?, TTargetType?>? setConverter = null)
    {
        return new DictionaryWrapper<TBaseType, TTargetType>(source, memberName, getConverter, setConverter);
    }
    
    private DictionaryWrapper(object source, string memberName, Func<TTargetType?, TBaseType?>? getConverter,
        Func<TBaseType?, TTargetType?>? setConverter) : base(source, memberName, getConverter, setConverter)
    {
        if (!typeof(IDictionary<int, TTargetType>).IsAssignableFrom(_member.MemberType))
            throw new Exception("Member is not of type IDictionary<int, TTargetType>");

        _dict = (IDictionary<int, TTargetType?>?)_member.GetValue(source);
    }

    private IDictionary<int, TTargetType?> GetDict()
    {
        if (_dict == null)
        {
            var instance = Activator.CreateInstance(_member.MemberType);
            _member.SetValue(_source, instance);

            _dict = instance as IDictionary<int, TTargetType?>;
        }

        return _dict;
    }

    public IEnumerator<KeyValuePair<int, TBaseType?>> GetEnumerator()
    {
        if (_dict == null)
            return Array.Empty<KeyValuePair<int, TBaseType?>>().Select(i => i).GetEnumerator();

        return _dict
            .Select(p => new KeyValuePair<int, TBaseType?>(p.Key, ConvertToBase(p.Value))).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(KeyValuePair<int, TBaseType?> item) => GetDict().Add(item.Key, ConvertToTarget(item.Value));

    public void Clear() => _dict?.Clear();

    public bool Contains(KeyValuePair<int, TBaseType?> item) =>
        _dict?.Contains(new KeyValuePair<int, TTargetType?>(item.Key, ConvertToTarget(item.Value))) ?? false;

    public void CopyTo(KeyValuePair<int, TBaseType?>[] array, int arrayIndex)
    {
        _dict?.Select(p => new KeyValuePair<int, TBaseType?>(p.Key, ConvertToBase(p.Value)))
            .ToArray().CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<int, TBaseType?> item)
    {
        return _dict?.Remove(new KeyValuePair<int, TTargetType?>(item.Key, ConvertToTarget(item.Value))) ?? false;
    }

    public int Count => _dict?.Count ?? 0;

    public bool IsReadOnly => _dict?.IsReadOnly ?? false;

    public void Add(int key, TBaseType? value) => GetDict().Add(key, ConvertToTarget(value));

    public bool ContainsKey(int key) => _dict != null && _dict.ContainsKey(key);

    public bool Remove(int key) => _dict?.Remove(key) ?? false;

    public bool TryGetValue(int key, out TBaseType? value)
    {
        if (_dict == null)
        {
            value = default;
            return false;
        }

        if (_dict.TryGetValue(key, out var result))
        {
            value = ConvertToBase(result);
            return true;
        }

        value = default;
        return false;
    }

    public TBaseType? this[int key]
    {
        get => _dict != null ? ConvertToBase(_dict[key]) : default;
        set => GetDict().Add(key, ConvertToTarget(value));
    }

    public ICollection<int> Keys => _dict != null ? _dict.Keys : [];

    public ICollection<TBaseType?> Values => _dict != null ? _dict.Values.Select(ConvertToBase).ToArray() : [];
}