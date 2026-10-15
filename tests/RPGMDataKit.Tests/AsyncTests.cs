using RPGMDataKit.Data.VXAce.Classes;
using RPGMDataKit.Diff;
using RPGMDataKit.Storages;
using RPGMDataKit.Storages.Abstraction;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>The methods that do not block the thread give the same files as the ones that do.</summary>
public sealed class AsyncTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "RPGMDataKit.Tests." + Guid.NewGuid().ToString("N"));

    public AsyncTests() => Directory.CreateDirectory(_folder);

    public void Dispose() => Directory.Delete(_folder, true);

    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private static string DataFolder(Version version) => version == Version.VXAce ? "Data" : "data";

    private static Storage Open(Version version, string project) =>
        version == Version.VXAce ? new VXAceStorage(project, "Data") : new MVStorage(project, "data");

    private string CopyProject(Version version, string name)
    {
        var project = Path.Combine(_folder, name);
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

    private string EmptyProject(Version version, string name)
    {
        var project = Path.Combine(_folder, name);
        Directory.CreateDirectory(Path.Combine(project, DataFolder(version)));

        return project;
    }

    private static void AssertSameFiles(string expected, string actual)
    {
        var names = Directory.GetFiles(expected, "*", SearchOption.AllDirectories)
            .Select(file => Path.GetRelativePath(expected, file)).Order().ToList();

        Assert.NotEmpty(names);
        Assert.Equal(names,
            Directory.GetFiles(actual, "*", SearchOption.AllDirectories).Select(file => Path.GetRelativePath(actual, file)).Order().ToList());
        Assert.All(names, name => Assert.True(
            File.ReadAllBytes(Path.Combine(expected, name)).AsSpan().SequenceEqual(File.ReadAllBytes(Path.Combine(actual, name))),
            $"{name} is different"));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task PreloadAndSaveWithoutBlockingGiveTheSameFiles(Version version)
    {
        var project = CopyProject(version, "project");

        var blocking = Open(version, project);
        blocking.Preload();
        var savedBlocking = EmptyProject(version, "saved-blocking");
        blocking.ChangeProjectPath(savedBlocking, DataFolder(version));
        blocking.SaveAll();

        var free = Open(version, project);
        await free.PreloadAsync();
        var savedFree = EmptyProject(version, "saved-free");
        free.ChangeProjectPath(savedFree, DataFolder(version));
        await free.SaveAllAsync();

        AssertSameFiles(savedBlocking, savedFree);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public async Task ALoadOfADumpWithoutBlockingGivesTheSameFiles(Version version)
    {
        var project = CopyProject(version, "project");
        var source = Open(version, project);
        var dump = Path.Combine(_folder, "dump");
        Directory.CreateDirectory(dump);
        await source.CreateYamlStorage().Dump(dump);

        var blocking = Open(version, EmptyProject(version, "blocking"));
        blocking.CreateYamlStorage().Load(dump);
        blocking.SaveAll();

        var free = Open(version, EmptyProject(version, "free"));
        await free.CreateYamlStorage().LoadAsync(dump);
        await free.SaveAllAsync(maxDegreeOfParallelism: 1);

        AssertSameFiles(Path.Combine(_folder, "blocking"), Path.Combine(_folder, "free"));
    }

    [Fact]
    public async Task AFileThatIsMissingIsAnErrorWithoutBlockingToo()
    {
        var storage = new MVStorage(EmptyProject(Version.Mv, "empty"));

        await Assert.ThrowsAsync<FileNotFoundException>(() => storage.PreloadAsync());
    }

    private Patch ANewMapPatch() => new() { Type = DiffType.Added, Map = new Patch.MapData { MapId = 1 } };

    [Fact]
    public async Task PatchesAreWrittenAndReadWithoutBlockingToo()
    {
        var storage = new MVStorage(CopyProject(Version.Mv, "project"));
        var patches = new List<Patch> { ANewMapPatch() };

        var blocking = Directory.CreateDirectory(Path.Combine(_folder, "text-blocking")).FullName;
        var free = Directory.CreateDirectory(Path.Combine(_folder, "text-free")).FullName;
        Differ.DumpPatchesAsText(blocking, patches, storage, storage);
        await Differ.DumpPatchesAsTextAsync(free, patches, storage, storage);

        AssertSameFiles(blocking, free);

        var json = Path.Combine(_folder, "patches.json");
        await Differ.DumpPatchesAsync(json, patches);
        Assert.Equal(patches.Count, (await Differ.LoadPatchesAsync(json)).Count);
        Assert.Equal(Differ.LoadPatches(json).Count, (await Differ.LoadPatchesAsync(json)).Count);
    }

    [Fact]
    public void TheFileOfTheMapsHasTheMaps()
    {
        var storage = new MVStorage(CopyProject(Version.Mv, "project"));
        var directory = Directory.CreateDirectory(Path.Combine(_folder, "text")).FullName;

        Differ.DumpPatchesAsText(directory, [ANewMapPatch()], storage, storage);

        var text = File.ReadAllText(Path.Combine(directory, "Maps.patch"));
        Assert.StartsWith("New map #1 ('" + storage.GetMapInfo(1)!.Name + "')", text);
        Assert.False(File.Exists(Path.Combine(directory, "Switches.patch")));
    }
}
