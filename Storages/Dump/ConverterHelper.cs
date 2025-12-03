using System.Reflection;
using RPGMakerLibCoreV2.Data.MV.Classes;

namespace RPGMakerLibCoreV2.Storages.Dump;

public static class ConverterHelper
{
    public interface IMember
    {
        public string Name { get; }
        
        public Type MemberType { get; }

        public void SetValue(object obj, object? value);

        public object? GetValue(object obj);
    }
    
    public class Member : IMember
    {
        public readonly MemberInfo MemberInfo;

        public Member(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
        }
        
        public string Name => MemberInfo.Name;

        public Type MemberType
        {
            get
            {
                if (MemberInfo.MemberType == MemberTypes.Property)
                    return (MemberInfo as PropertyInfo)!.PropertyType;
                if (MemberInfo.MemberType == MemberTypes.Field)
                    return (MemberInfo as FieldInfo)!.FieldType;
                
                throw new Exception("Unsupported member type");
            }
        }

        public void SetValue(object obj, object? value)
        {
            if (MemberInfo.MemberType == MemberTypes.Property)
                (MemberInfo as PropertyInfo)!.SetValue(obj, value);
            else if (MemberInfo.MemberType == MemberTypes.Field)
                (MemberInfo as FieldInfo)!.SetValue(obj, value);
            else
                throw new Exception("Unsupported member type");
        }

        public object? GetValue(object obj)
        {
            if (MemberInfo.MemberType == MemberTypes.Property)
                return (MemberInfo as PropertyInfo)!.GetValue(obj);
            if (MemberInfo.MemberType == MemberTypes.Field)
                return (MemberInfo as FieldInfo)!.GetValue(obj);

            throw new Exception("Unsupported member type");;
        }
    }

    public class MemberWithAttribute : IMember
    {
        public MemberWithAttribute(MemberInfo memberInfo, Attribute attribute)
        {
            _member = new(memberInfo);
            Attribute = attribute;
        }

        public MemberWithAttribute(Member member, Attribute attribute)
        {
            _member = member;
            Attribute = attribute;
        }

        public Attribute Attribute { get; init; }

        private Member _member;
        
        public T? AttributeAs<T>() where T : Attribute => Attribute as T;
        
        public string Name => _member.Name;
        
        public Type MemberType => _member.MemberType;
        
        public void SetValue(object obj, object? value) => _member.SetValue(obj, value);

        public object? GetValue(object obj) => _member.GetValue(obj);
    }

    public static Dictionary<Type, string> TaggedTypes { get; } = new()
    {
        [typeof(SoundData)] = "SoundData",
        [typeof(MoveRoute)] = "MoveRoute",
        [typeof(MoveCommand)] = "MoveCommand",
    };
    
    private static Dictionary<Type, Dictionary<Type, IEnumerable<MemberWithAttribute>>> _membersWithAttributesCache = new();
    private static Dictionary<Type, IEnumerable<Member>> _membersCache = new();
    
    public static IEnumerable<MemberWithAttribute> GetMembersWithAttribute<T>(Type type) where T : Attribute
    {
        if (_membersWithAttributesCache.TryGetValue(type, out var byAttribute))
        {
            if (byAttribute.TryGetValue(typeof(T), out var cachedMembers))
                return cachedMembers;
        }
        else
            _membersWithAttributesCache[type] = new();

        List<MemberWithAttribute> members = new();

        foreach (var member in GetMembers(type))
        {
            var attr = member.MemberInfo.GetCustomAttribute<T>();
            if (attr != null)
                members.Add(new(member, attr));
        }

        _membersWithAttributesCache[type][typeof(T)] = members;

        return members;
    }

    public static IEnumerable<Member> GetMembers(Type type)
    {
        if (_membersCache.TryGetValue(type, out var members))
            return members;

        members = GetMembersRecursive(type);

        _membersCache[type] = members;

        return members;
    }

    private static IEnumerable<Member> GetMembersRecursive(Type type)
    {
        List<Member> members = new();
        members.AddRange(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                        BindingFlags.DeclaredOnly).Select(m => new Member(m)));
        members.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                            BindingFlags.DeclaredOnly).Select(m => new Member(m)));
        if (type.BaseType != null)
            members.AddRange(GetMembersRecursive(type.BaseType));

        return members;
    }
}