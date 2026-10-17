using System.Collections.Concurrent;
using System.Reflection;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Attributes;

namespace RPGMDataKit.Data.Structures.Internal;

/// <summary>Which properties of a class are the ones that its maker does not have; found once for each class.</summary>
internal static class OptionalProperties
{
    private static readonly ConcurrentDictionary<Type, (HashSet<string> All, HashSet<string> Missing)> Classes = new();

    public static bool Has(Type type, string propertyName)
    {
        var (all, missing) = Classes.GetOrAdd(type, Scan);
        if (!all.Contains(propertyName))
            throw new ArgumentException($"{type.Name} has no property {propertyName}.", nameof(propertyName));

        return !missing.Contains(propertyName);
    }

    private static (HashSet<string>, HashSet<string>) Scan(Type type)
    {
        HashSet<string> all = [], missing = [];
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            all.Add(property.Name);
            if (property.GetCustomAttribute<DoesNotExistInVersionAttribute>() != null)
                missing.Add(property.Name);
        }

        return (all, missing);
    }
}
