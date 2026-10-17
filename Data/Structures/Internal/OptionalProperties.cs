using System.Collections.Concurrent;
using System.Reflection;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Attributes;

namespace RPGMDataKit.Data.Structures.Internal;

/// <summary>Which properties of a class are the ones that a maker does not have (<see cref="DoesNotExistInVersionAttribute"/>); found once for each class.</summary>
internal static class OptionalProperties
{
    // for each property of a class: null if it is always there, else the makers that do not have it (none: the class does not have it)
    private sealed record ClassInfo(Dictionary<string, IReadOnlyList<StorageVersion>?> Properties, bool AnyMarked);

    private static readonly ConcurrentDictionary<Type, ClassInfo> Classes = new();

    /// <summary>
    /// Whether the class has the property. With the maker of the object (null if it is not known yet) for a class that is the same for both makers.
    /// </summary>
    /// <exception cref="ArgumentException">The class has no such property.</exception>
    public static bool Has(Type type, string propertyName, StorageVersion? version = null)
    {
        if (!Classes.GetOrAdd(type, Scan).Properties.ContainsKey(propertyName))
            throw new ArgumentException($"{type.Name} has no property {propertyName}.", nameof(propertyName));

        return !IsMissing(type, propertyName, version);
    }

    /// <summary>
    /// True if the name is a property of the class that the maker does not have. A name that is not a property (the caller was a method) is not.
    /// Quick for a class that has no such properties, since it is asked for every parameter that is read or written.
    /// </summary>
    public static bool IsMissing(Type type, string? propertyName, StorageVersion? version)
    {
        if (propertyName == null)
            return false;

        var info = Classes.GetOrAdd(type, Scan);
        if (!info.AnyMarked || !info.Properties.TryGetValue(propertyName, out var missing) || missing == null)
            return false;

        // marked without makers: the class does not have the property; marked with makers: those do not have it, and an unknown maker may
        return missing.Count == 0 || (version != null && missing.Contains(version.Value));
    }

    private static ClassInfo Scan(Type type)
    {
        var properties = new Dictionary<string, IReadOnlyList<StorageVersion>?>();
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            properties[property.Name] = property.GetCustomAttribute<DoesNotExistInVersionAttribute>()?.Versions;

        return new ClassInfo(properties, properties.Values.Any(v => v != null));
    }
}
