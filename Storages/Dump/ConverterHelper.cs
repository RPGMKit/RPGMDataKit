using System.Reflection;
using RPGMakerLibCoreV2.Data.MV.Classes;

namespace RPGMakerLibCoreV2.Storages.Dump;

public static class ConverterHelper
{
    public class Member
    {
        public required Attribute Attribute { get; init; }
        public required MemberInfo MemberInfo { get; init; }

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

        public T? AttributeAs<T>() where T : Attribute => Attribute as T;
    }

    public static Dictionary<Type, string> TaggedTypes { get; } = new()
    {
        [typeof(SoundData)] = "SoundData",
        [typeof(MoveRoute)] = "MoveRoute",
        [typeof(MoveCommand)] = "MoveCommand",
    };
    
    private static Dictionary<Type, Dictionary<Type, IEnumerable<Member>>> _cache = new();

    public static IEnumerable<Member> GetMembersWithAttribute<T>(Type type) where T : Attribute
    {
        if (_cache.TryGetValue(type, out var byAttribute))
        {
            if (byAttribute.TryGetValue(typeof(T), out var cachedMembers))
                return cachedMembers;
        }
        else
            _cache[type] = new();

        List<Member> members = new();

        foreach (var member in GetMembers(type))
        {
            var attr = member.GetCustomAttribute<T>();
            if (attr != null)
                members.Add(new() { Attribute = attr, MemberInfo = member });
        }

        _cache[type][typeof(T)] = members;

        return members;
    }

    private static IEnumerable<MemberInfo> GetMembers(Type type)
    {
        List<MemberInfo> members = new();
        members.AddRange(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                        BindingFlags.DeclaredOnly));
        members.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                            BindingFlags.DeclaredOnly));
        if (type.BaseType != null)
            members.AddRange(GetMembers(type.BaseType));

        return members;
    }
}