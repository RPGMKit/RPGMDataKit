using RPGMakerLibCoreV2.Storages.Dump;

namespace RPGMakerLibCoreV2.Data.Structures.Internal.Wrappers;

public abstract class AbstractWrapper<TBaseType, TTargetType>
{
    protected ConverterHelper.Member _member;
    
    protected readonly object _source;
    
    private Func<TTargetType?, TBaseType?>? _getConverter;
    
    private Func<TBaseType?, TTargetType?>? _setConverter;

    protected AbstractWrapper(object source, string memberName, Func<TTargetType?, TBaseType?>? getConverter,
        Func<TBaseType?, TTargetType?>? setConverter)
    {
        _source = source;
        _member = GetMemberInfo(source.GetType(), memberName) ?? throw new Exception("Member not found in source object");

        _getConverter = getConverter;
        _setConverter = setConverter;
    }
    
    private static Dictionary<Type, Dictionary<string, ConverterHelper.Member?>> _memberInfos = new();

    private static ConverterHelper.Member? GetMemberInfo(Type t, string memberName)
    {
        if (_memberInfos.TryGetValue(t, out var result))
            if (!result.TryGetValue(memberName, out var member))
                return member;

        var m = ConverterHelper.GetMembers(t).FirstOrDefault(m => m.Name == memberName);

        if (!_memberInfos.TryGetValue(t, out var info))
            _memberInfos[t] = new() { [memberName] = m };
        else
            info[memberName] = m;

        return m;
    }

    protected TBaseType? ConvertToBase(TTargetType? value)
    {
        if (_getConverter != null)
            return _getConverter.Invoke(value);

        return (TBaseType?)(object?)value;
    }

    protected TTargetType? ConvertToTarget(TBaseType? value)
    {
        if (_setConverter != null)
            return _setConverter.Invoke(value);

        return (TTargetType?)(object?)value;
    }
}
