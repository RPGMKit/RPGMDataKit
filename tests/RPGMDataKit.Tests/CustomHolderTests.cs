using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGMDataKit.Data;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Storages;
using RPGMDataKit.Storages.Dump;
using RPGMDataKit.Storages.Abstraction;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>Tables of your own (<see cref="Storage.AddList{T}"/>, <see cref="Storage.AddDictionary{T}"/>, <see cref="Storage.AddEntity{T}"/>).</summary>
public sealed class CustomHolderTests : IDisposable
{
    public class Quest : JsonEntity, IHasId
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; } = "";
    }

    public class Settings : JsonEntity
    {
        [JsonProperty("volume")]
        public int Volume { get; set; } = 7;
    }

    /// <summary>Does not keep the members it does not describe.</summary>
    public class Plain : IHasId
    {
        public int Id { get; set; }
    }

    private readonly string _folder = Path.Combine(Path.GetTempPath(), "RPGMDataKit.Tests." + Guid.NewGuid().ToString("N"));

    public CustomHolderTests() => Directory.CreateDirectory(Path.Combine(_folder, "data"));

    public void Dispose() => Directory.Delete(_folder, true);

    private string DataFile(string name) => Path.Combine(_folder, "data", name);

    private void Write(string name, string json) => File.WriteAllText(DataFile(name), json);

    [Fact]
    public void AListIsReadByIndexAndSavedWithWhatTheClassDoesNotDescribe()
    {
        Write("Quests.json", """[null,{"id":1,"title":"A","extra":7},{"id":2,"title":"B"}]""");
        var storage = new MVStorage(_folder);

        var quests = storage.AddList<Quest>("Quests");
        quests.Get(1)!.Title = "A2";
        var id = quests.Add(new Quest { Title = "C" });
        storage.SaveAll();

        Assert.Equal(3, id);
        Assert.Equal(3, quests.Get(3)!.Id);
        var saved = JArray.Parse(File.ReadAllText(DataFile("Quests.json")));
        Assert.Equal(JTokenType.Null, saved[0].Type);
        Assert.Equal("A2", (string?)saved[1]["title"]);
        Assert.Equal(7, (int?)saved[1]["extra"]);
        Assert.Equal(3, (int?)saved[3]["id"]);
    }

    [Fact]
    public void AnIdThatDiffersFromTheIndexIsReportedAndLeftAsItIs()
    {
        Write("Quests.json", """[null,{"id":1,"title":"A"},{"id":5,"title":"B"}]""");
        var storage = new MVStorage(_folder);
        var mismatches = new List<IdMismatch>();
        storage.IdMismatched += mismatches.Add;

        var quests = storage.AddList<Quest>("Quests");
        var second = quests.Get(2)!;

        var mismatch = Assert.Single(mismatches);
        Assert.Equal(("Quests.json", 2, 5), (mismatch.File, mismatch.Key, mismatch.EntityId));
        Assert.Same(second, mismatch.Entity);
        Assert.Equal(5, second.Id);
    }

    [Fact]
    public void ADictionaryIsReadByKeyAndReportsAnIdThatDiffers()
    {
        Write("Stats.json", """{"3":{"id":3,"title":"x"},"4":{"id":9,"title":"y"}}""");
        var storage = new MVStorage(_folder);
        var mismatches = new List<IdMismatch>();
        storage.IdMismatched += mismatches.Add;

        var stats = storage.AddDictionary<Quest>("Stats");

        Assert.Equal("y", stats.Get(4)!.Title);
        Assert.Equal(4, Assert.Single(mismatches).Key);
    }

    [Fact]
    public void ASingleEntityKeepsWhatTheClassDoesNotDescribe()
    {
        Write("Settings.json", """{"volume":3,"other":{"a":1}}""");
        var storage = new MVStorage(_folder);

        var settings = storage.AddEntity<Settings>("Settings");
        settings.Get().Volume = 9;
        storage.SaveAll();

        var saved = JObject.Parse(File.ReadAllText(DataFile("Settings.json")));
        Assert.Equal(9, (int?)saved["volume"]);
        Assert.Equal(1, (int?)saved["other"]!["a"]);
    }

    [Fact]
    public void AnOptionalTableStartsEmptyWhenTheFileIsMissingAndTheFileIsWrittenOnSave()
    {
        var storage = new MVStorage(_folder);

        var quests = storage.AddList<Quest>("Quests", optional: true);
        var settings = storage.AddEntity<Settings>("Settings", optional: true);
        quests.Add(new Quest { Title = "A" });
        storage.SaveAll();

        Assert.Equal(7, settings.Get().Volume);
        Assert.Equal("A", (string?)JArray.Parse(File.ReadAllText(DataFile("Quests.json")))[1]["title"]);
        Assert.True(File.Exists(DataFile("Settings.json")));
    }

    [Fact]
    public void ATableThatIsNotOptionalNeedsItsFile()
    {
        var storage = new MVStorage(_folder);

        var quests = storage.AddList<Quest>("Quests");

        Assert.Throws<FileNotFoundException>(() => quests.Get(1));
    }

    [Fact]
    public void ATypeThatWouldLoseMembersIsRefused()
    {
        var storage = new MVStorage(_folder);

        Assert.Throws<ArgumentException>(() => storage.AddList<Plain>("Plain"));
    }

    [Fact]
    public void ANameThatIsAlreadyUsedIsRefused()
    {
        var storage = new MVStorage(_folder);
        storage.AddList<Quest>("Quests", optional: true);

        Assert.Throws<ArgumentException>(() => storage.AddList<Quest>("quests", optional: true));
        Assert.Throws<ArgumentException>(() => storage.AddList<Quest>("Actors", optional: true));
        Assert.Throws<ArgumentException>(() => storage.AddList<Quest>("a/b", optional: true));
    }

    [Fact]
    public void TheStandardTablesAreReportedToo()
    {
        Write("Actors.json", """[null,{"id":4,"name":"A"}]""");
        var storage = new MVStorage(_folder);
        var mismatches = new List<IdMismatch>();
        storage.IdMismatched += mismatches.Add;

        storage.Actors.Get(1);

        Assert.Equal(("Actors.json", 1, 4), (mismatches.Single().File, mismatches.Single().Key, mismatches.Single().EntityId));
    }

    private void CopyEmptyMvProject(string project)
    {
        var data = Path.Combine(project, "data");
        Directory.CreateDirectory(data);
        foreach (var file in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Projects", "Mv", "data")))
            File.Copy(file, Path.Combine(data, Path.GetFileName(file)), true);
    }

    [Fact]
    public async Task AddedHoldersAreDumpedAndLoadedWithTheStandardOnes()
    {
        CopyEmptyMvProject(_folder);
        Write("Quests.json", """[null,{"id":1,"title":"A","extra":7},{"id":2,"title":"B"}]""");
        Write("Stats.json", """{"3":{"id":3,"title":"x"}}""");
        Write("Settings.json", """{"volume":3,"other":{"a":1}}""");
        var storage = new MVStorage(_folder);
        storage.AddList<Quest>("Quests");
        storage.AddDictionary<Quest>("Stats");
        storage.AddEntity<Settings>("Settings");
        var dump = Path.Combine(_folder, "dump");
        Directory.CreateDirectory(dump);

        await storage.CreateYamlStorage().Dump(dump);

        Assert.True(File.Exists(Path.Combine(dump, "Quests", "1.yaml")));
        Assert.True(File.Exists(Path.Combine(dump, "Quests", "2.yaml")));
        Assert.True(File.Exists(Path.Combine(dump, "Stats", "3.yaml")));
        Assert.True(File.Exists(Path.Combine(dump, "Settings.yaml")));
        Assert.True(File.Exists(Path.Combine(dump, "Actors", "1.yaml")));

        var other = Path.Combine(Path.GetTempPath(), "RPGMDataKit.Tests." + Guid.NewGuid().ToString("N"));
        try
        {
            CopyEmptyMvProject(other);
            var loaded = new MVStorage(other);
            var quests = loaded.AddList<Quest>("Quests", optional: true);
            var stats = loaded.AddDictionary<Quest>("Stats", optional: true);
            var settings = loaded.AddEntity<Settings>("Settings", optional: true);

            loaded.CreateYamlStorage().Load(dump);
            loaded.SaveAll();

            Assert.Equal("A", quests.Get(1)!.Title);
            Assert.Equal("B", quests.Get(2)!.Title);
            Assert.Equal("x", stats.Get(3)!.Title);
            Assert.Equal(3, settings.Get().Volume);
            var saved = JArray.Parse(File.ReadAllText(Path.Combine(other, "data", "Quests.json")));
            Assert.Equal(7, (int?)saved[1]["extra"]);
        }
        finally
        {
            Directory.Delete(other, true);
        }
    }

    [Fact]
    public void ALoadWithoutADumpOfAnAddedHolderLeavesItAsItIs()
    {
        CopyEmptyMvProject(_folder);
        Write("Settings.json", """{"volume":3}""");
        var storage = new MVStorage(_folder);
        var settings = storage.AddEntity<Settings>("Settings");
        var dump = Path.Combine(_folder, "dump");
        Directory.CreateDirectory(dump);

        storage.CreateYamlStorage().Load(dump);

        Assert.Equal(3, settings.Get().Volume);
    }

    [Fact]
    public void AddedHoldersAreFoundByName()
    {
        var storage = new MVStorage(_folder);
        var quests = storage.AddList<Quest>("Quests", optional: true);
        var settings = storage.AddEntity<Settings>("Settings", optional: true);

        Assert.Same(quests, storage.GetHolder<DataLookupHolder<Quest>>("quests"));
        Assert.Same(settings, storage.GetHolder<DataHolder<Settings>>("SETTINGS"));
        Assert.Contains("Quests", storage.HolderNames);
        Assert.True(storage.TryGetHolder<DataLookupHolder<Quest>>("Quests", out var found));
        Assert.Same(quests, found);
        Assert.False(storage.TryGetHolder<DataLookupHolder<Quest>>("Settings", out _));
        Assert.False(storage.TryGetHolder<DataLookupHolder<Quest>>("Nothing", out _));
        Assert.Throws<KeyNotFoundException>(() => storage.GetHolder<DataLookupHolder<Quest>>("Nothing"));
        Assert.Throws<InvalidCastException>(() => storage.GetHolder<DataLookupHolder<Quest>>("Settings"));
    }

    [Theory]
    [InlineData(typeof(MVStorage))]
    [InlineData(typeof(VXAceStorage))]
    public void TheStandardHoldersAreFoundByNameToo(Type type)
    {
        var storage = (Storage)Activator.CreateInstance(type, _folder, type == typeof(VXAceStorage) ? "Data" : "data")!;

        Assert.Same(storage.Actors, storage.GetHolder<DataLookupHolder<IActor>>("actors"));
        Assert.Same(storage.Skills, storage.GetHolder<DataLookupHolder<ISkill>>("Skills"));
        Assert.Same(storage.MapInfos, storage.GetHolder<DataLookupHolder<IMapInfo>>("MapInfos"));
        Assert.Same(storage.System, storage.GetHolder<DataHolder<ISystem>>("System"));
        Assert.Same(storage.Maps, storage.GetHolder<MapsHolder>("Maps"));
        Assert.Equal(15, storage.HolderNames.Count(n => n != "Scripts"));
    }

    [Fact]
    public void AnAddedHolderCannotTakeTheNameOfTheMaps()
    {
        var storage = new MVStorage(_folder);

        Assert.Throws<ArgumentException>(() => storage.AddList<Quest>("maps", optional: true));
    }

    [Fact]
    public void TheIdOfAVxAceMapInfoIsItsKeyBecauseTheFileDoesNotKeepIt()
    {
        var source = Path.Combine(AppContext.BaseDirectory, "Projects", "VXAce", "Data");
        var data = Path.Combine(_folder, "Data");
        Directory.CreateDirectory(data);
        File.Copy(Path.Combine(source, "MapInfos.rvdata2"), Path.Combine(data, "MapInfos.rvdata2"));
        var storage = new VXAceStorage(_folder, "Data");
        var mismatches = new List<IdMismatch>();
        storage.IdMismatched += mismatches.Add;

        var maps = storage.MapInfos.Get();

        Assert.NotEmpty(maps);
        Assert.All(maps, pair => Assert.Equal(pair.Key, pair.Value!.Id));
        Assert.Empty(mismatches);
    }

    [Fact]
    public void VxAceDoesNotHaveTablesOfYourOwnYet()
    {
        var storage = new VXAceStorage(_folder, "data");

        Assert.Throws<NotSupportedException>(() => storage.AddList<Quest>("Quests", optional: true));
    }
}
