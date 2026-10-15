using System.Reflection;
using Newtonsoft.Json.Linq;
using RPGMDataKit.Data.VXAce.Classes;
using RPGMDataKit.Storages;
using RPGMDataKit.Storages.Abstraction;
using RPGMDataKit.Storages.Dump;
using RPGMDataKit.Storages.Serialization;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// The data of an empty project of each editor (<c>Projects</c>) is loaded and saved: nothing may be lost, and the only members
/// that the classes do not describe are the ones that the editors themselves leave behind (listed below).
/// </summary>
public sealed class ProjectFilesTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "RPGMDataKit.Tests." + Guid.NewGuid().ToString("N"));

    public ProjectFilesTests() => Directory.CreateDirectory(_folder);

    public void Dispose() => Directory.Delete(_folder, true);

    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    /// <summary>The members of the data files that the editors write and do not use; they stay in the unknown fields and are saved back.</summary>
    private static readonly Dictionary<string, string[]> LeftOvers = new()
    {
        ["MV"] =
        [
            $"{typeof(Data.MV.Classes.State).FullName}.description",
            $"{typeof(Data.MV.Classes.State).FullName}.releaseByDamage",
        ],
        ["VXAce"] =
        [
            $"{typeof(Data.VXAce.Classes.Enemy).FullName}.@eva",
            $"{typeof(Data.VXAce.Classes.Enemy).FullName}.@hit",
            $"{typeof(Data.VXAce.Classes.State).FullName}.@release_by_damage",
        ],
    };

    private string Copy(Version version)
    {
        var project = Path.Combine(_folder, "project");
        var source = Path.Combine(AppContext.BaseDirectory, "Projects", version.Name);
        foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            var target = Path.Combine(project, Path.GetRelativePath(source, file));
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(file, target);
        }

        // the standard scripts of VX Ace are not kept in the repository, an empty list stands in for them
        if (version == Version.VXAce)
            version.Serializer.Save(Path.Combine(project, "Data", "Scripts.rvdata2"), new List<Script>());

        return project;
    }

    private static Storage Open(Version version, string project) =>
        version == Version.VXAce ? new VXAceStorage(project, "Data") : new MVStorage(project, "data");

    private static string DataFolder(Version version) => version == Version.VXAce ? "Data" : "data";

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task SavedProjectHasTheSameData(Version version)
    {
        var project = Copy(version);
        var saved = Path.Combine(_folder, "saved");
        var storage = Open(version, project);
        storage.Preload();

        var dumpBefore = Path.Combine(_folder, "dump-before");
        await Dump(storage, dumpBefore);

        storage.ChangeProjectPath(saved, DataFolder(version));
        Directory.CreateDirectory(Path.Combine(saved, DataFolder(version)));
        storage.SaveAll();

        if (version == Version.VXAce)
        {
            // the bytes of a Ruby Marshal file are not the only way to write the same data: the loaded data is compared
            var reopened = Open(version, saved);
            reopened.Preload();
            var dumpAfter = Path.Combine(_folder, "dump-after");
            await Dump(reopened, dumpAfter);

            Assert.NotEmpty(Directory.GetFiles(dumpBefore, "*", SearchOption.AllDirectories));
            var changed = Directory.GetFiles(dumpBefore, "*", SearchOption.AllDirectories)
                .Select(file => Path.GetRelativePath(dumpBefore, file))
                .Where(name => !File.Exists(Path.Combine(dumpAfter, name)) ||
                               File.ReadAllText(Path.Combine(dumpBefore, name)) != File.ReadAllText(Path.Combine(dumpAfter, name)))
                .ToList();
            Assert.Empty(changed);
            return;
        }

        var different = new List<string>();
        foreach (var original in Directory.GetFiles(Path.Combine(project, DataFolder(version))))
        {
            var name = Path.GetFileName(original);
            var copy = Path.Combine(saved, DataFolder(version), name);
            if (name.StartsWith("Scripts"))
                continue;

            Assert.True(File.Exists(copy), $"{name} was not saved");
            if (!SameData(version, original, copy))
                different.Add(name);
        }

        Assert.Empty(different);
    }

    private static async Task Dump(Storage storage, string folder)
    {
        Directory.CreateDirectory(folder);
        await storage.CreateYamlStorage().Dump(folder);
    }

    /// <summary>The same JSON in the file of MV (the order of the keys does not matter, nothing may be missing).</summary>
    private static bool SameData(Version version, string original, string copy)
    {
        return Contains(JToken.Parse(File.ReadAllText(copy)), JToken.Parse(File.ReadAllText(original)));
    }

    /// <summary>
    /// Whether everything of <paramref name="expected"/> is in <paramref name="actual"/> with the same values. The library adds
    /// members that the editor does not write (the conditions of the timings of animations, the indent of the commands of a route, the
    /// encryption members of the system) and writes whole numbers as floats where the engine reads them the same; that is not a loss.
    /// </summary>
    private static bool Contains(JToken actual, JToken expected)
    {
        switch (expected)
        {
            case JObject o:
                return actual is JObject a && o.Properties().All(p => a.TryGetValue(p.Name, out var value) && Contains(value, p.Value));
            case JArray e:
                return actual is JArray array && array.Count == e.Count && e.Zip(array).All(pair => Contains(pair.Second, pair.First));
            case JValue v when v.Type is JTokenType.Integer or JTokenType.Float:
                return actual is JValue { Type: JTokenType.Integer or JTokenType.Float } n && n.Value<double>() == v.Value<double>();
            default:
                return JToken.DeepEquals(actual, expected);
        }
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void OnlyTheLeftOversOfTheEditorsAreUnknown(Version version)
    {
        var storage = Open(version, Copy(version));
        storage.Preload();

        var unknown = new List<string>();
        var seen = new HashSet<object>(ReferenceEqualityComparer.Instance);
        foreach (var field in typeof(Storage).GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            var holder = field.GetValue(storage);
            var get = field.FieldType.GetMethod("Get", Type.EmptyTypes);
            if (holder != null && get != null)
                RoundTripTests.FindUnknownFields(get.Invoke(holder, null), field.Name, unknown, seen);
        }

        foreach (var id in storage.MapInfos.Get().Keys)
            RoundTripTests.FindUnknownFields(storage.Maps.Get(id), $"Map{id}", unknown, seen);

        Assert.Equal(LeftOvers[version.Name], unknown.Distinct().Order().ToArray());
    }
}
