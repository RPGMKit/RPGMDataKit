using System.Collections;

namespace RPGMakerLibCoreV2.Data.Structures.Internal.Wrappers;

public class ListWrapper<TBaseType, TTargetType> : AbstractWrapper<TBaseType, TTargetType>, IList<TBaseType?>
{
    private IList<TTargetType?>? _list;

    public static ListWrapper<TBaseType, TTargetType> Create(object source, string memberName,
        Func<TTargetType?, TBaseType?>? getConverter = null,
        Func<TBaseType?, TTargetType?>? setConverter = null)
    {
        return new ListWrapper<TBaseType, TTargetType>(source, memberName, getConverter, setConverter);
    }

    private ListWrapper(object source, string memberName, Func<TTargetType?, TBaseType?>? getConverter,
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

    public IEnumerator<TBaseType?> GetEnumerator()
    {
        if (_list == null)
            return Array.Empty<TBaseType?>().Select(e => e).GetEnumerator();

        return _list.Where(i => i != null).Select(ConvertToBase).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(TBaseType? item) => GetList().Add(ConvertToTarget(item));

    public void Clear() => _list?.Clear();

    public bool Contains(TBaseType? item) => _list?.Contains(ConvertToTarget(item)) ?? false;

    public void CopyTo(TBaseType?[] array, int arrayIndex) =>
        _list?.Select(ConvertToBase).ToList().CopyTo(array, arrayIndex);

    public bool Remove(TBaseType? item) => _list?.Remove(ConvertToTarget(item)) ?? false;

    public int Count => _list?.Count ?? 0;

    public bool IsReadOnly => _list?.IsReadOnly ?? false;

    public int IndexOf(TBaseType? item) => _list?.IndexOf(ConvertToTarget(item)) ?? -1;

    public void Insert(int index, TBaseType? item) => GetList().Insert(index, ConvertToTarget(item));

    public void RemoveAt(int index) => _list?.RemoveAt(index);

    public TBaseType? this[int index]
    {
        get => _list != null ? ConvertToBase(_list[index]) : default;
        set => GetList()[index] = ConvertToTarget(value);
    }
}
