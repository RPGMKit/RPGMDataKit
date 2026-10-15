using System.Collections;
using System.Reflection;
using RPGMDataKit.Data.Structures.Exceptions;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// A new entity is saved, loaded and saved again: the file must come out the same, no member may be lost and none may be left
/// to the unknown fields (a member of the format that the class does not describe).
/// </summary>
public sealed class RoundTripTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "RPGMDataKit.Tests." + Guid.NewGuid().ToString("N"));

    public RoundTripTests() => Directory.CreateDirectory(_folder);

    public void Dispose() => Directory.Delete(_folder, true);

    public static IEnumerable<object[]> Cases() => Version.FactoryCases();

    [Theory]
    [MemberData(nameof(Cases))]
    public void SavedAgainGivesTheSameFile(Version version, Type entityInterface)
    {
        var first = Path.Combine(_folder, "first" + version.Serializer.FileExtension);
        var second = Path.Combine(_folder, "second" + version.Serializer.FileExtension);

        version.Save(version.New(entityInterface), first);
        var loaded = version.Load(entityInterface, first);
        version.Save(loaded, second);

        Assert.Equal(File.ReadAllBytes(first), File.ReadAllBytes(second));
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void LoadedEntityHasNoUnknownFields(Version version, Type entityInterface)
    {
        var path = Path.Combine(_folder, "entity" + version.Serializer.FileExtension);
        version.Save(version.New(entityInterface), path);

        var unknown = new List<string>();
        FindUnknownFields(version.Load(entityInterface, path), "", unknown, new HashSet<object>(ReferenceEqualityComparer.Instance));

        Assert.Empty(unknown);
    }

    /// <summary>
    /// Both makers keep what the class does not describe in a dictionary (<c>_unknownFields</c>); a new entity must not
    /// need it, because every member that the factory sets is described.
    /// </summary>
    internal static void FindUnknownFields(object? entity, string path, List<string> found, HashSet<object> seen)
    {
        if (entity is null or string || entity.GetType().IsPrimitive || entity.GetType().IsEnum)
            return;

        if (!entity.GetType().IsValueType && !seen.Add(entity))
            return;

        if (entity is IDictionary dictionary)
        {
            foreach (var value in dictionary.Values)
                FindUnknownFields(value, path, found, seen);
            return;
        }

        if (entity is IEnumerable items)
        {
            var index = 0;
            foreach (var item in items)
                FindUnknownFields(item, $"{path}[{index++}]", found, seen);
            return;
        }

        var type = entity.GetType();
        if (type.Namespace?.StartsWith("RPGMDataKit.Data") != true)
            return;

        for (var t = type; t != null; t = t.BaseType)
        {
            var field = t.GetField("_unknownFields", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (field?.GetValue(entity) is IDictionary { Count: > 0 } unknown)
                found.AddRange(unknown.Keys.Cast<object>().Select(key => $"{type.FullName}.{key}"));
        }

        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.GetIndexParameters().Length == 0))
        {
            object? value;
            try
            {
                value = property.GetValue(entity);
            }
            catch (TargetInvocationException exception) when (exception.InnerException is DoesNotExistInVersionException)
            {
                continue; // a member that the other maker has and this one does not
            }

            FindUnknownFields(value, $"{path}.{property.Name}", found, seen);
        }

        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            FindUnknownFields(field.GetValue(entity), $"{path}.{field.Name}", found, seen);
    }
}
