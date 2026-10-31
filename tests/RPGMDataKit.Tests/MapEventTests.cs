using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.VXAce.Classes;
using RPGMDataKit.Storages;
using RPGMDataKit.Storages.Abstraction;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary><see cref="MapEventExtensions.CreateEvent"/>: a new event on a map, with the first page made by a builder or empty.</summary>
public sealed class MapEventTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "RPGMDataKit.Tests." + Guid.NewGuid().ToString("N"));

    public MapEventTests() => Directory.CreateDirectory(_folder);

    public void Dispose() => Directory.Delete(_folder, true);

    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private string Project(Version v)
    {
        var project = Path.Combine(_folder, "project");
        var source = Path.Combine(AppContext.BaseDirectory, "Projects", v.Name);
        foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            var target = Path.Combine(project, Path.GetRelativePath(source, file));
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(file, target);
        }

        if (v == Version.VXAce)
            v.Serializer.Save(Path.Combine(project, "Data", "Scripts.rvdata2"), new List<Script>());

        return project;
    }

    private static Storage Open(Version v, string project) => v == Version.VXAce ? new VXAceStorage(project, "Data") : new MVStorage(project, "data");

    private static IMap NewMap(Storage storage) => storage.CreateMap("Cave", 10, 8).Map;

    [Theory]
    [MemberData(nameof(Versions))]
    public void AnEventGetsTheNextNumberAPlaceAndANameByItsNumber(Version v)
    {
        var map = NewMap(Open(v, Project(v)));

        var first = map.CreateEvent(3, 4);
        var second = map.CreateEvent(5, 6);

        Assert.Equal((1, "EV001", 3, 4), (first.Id, first.Name, first.X, first.Y));
        Assert.Equal((2, "EV002", 5, 6), (second.Id, second.Name, second.X, second.Y));
        Assert.Same(first, map.Events[1]);
        Assert.Same(second, map.Events[2]);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheNumberIsAfterTheHighestOneAndAGivenNameIsKept(Version v)
    {
        var map = NewMap(Open(v, Project(v)));
        map.Events[7] = null;
        map.CreateEvent(0, 0);
        map.Events.Remove(1);

        var created = map.CreateEvent(1, 1, "Door");

        // the empty slot 7 counts as taken, the removed number 1 is not reused
        Assert.Equal((9, "Door"), (created.Id, created.Name));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void WithoutABuilderThePageIsTheOneOfANewEvent(Version v)
    {
        var storage = Open(v, Project(v));
        var map = NewMap(storage);

        var created = map.CreateEvent(0, 0);
        var fresh = storage.New<IEvent>();

        var page = Assert.Single(created.Pages)!;
        var freshPage = fresh.Pages[0]!;
        Assert.Equal((freshPage.Trigger, freshPage.MoveSpeed, freshPage.MoveFrequency, freshPage.PriorityType), (page.Trigger, page.MoveSpeed, page.MoveFrequency, page.PriorityType));
        Assert.Equal(freshPage.NamedCommands.Select(c => c.Code), page.NamedCommands.Select(c => c.Code));
        Assert.Equal([0], page.NamedCommands.Select(c => c.Code));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheBuilderMakesTheCommandsOfTheFirstPage(Version v)
    {
        var map = NewMap(Open(v, Project(v)));

        var created = map.CreateEvent(2, 2, build: b => b
            .Text("Hello")
            .IfSwitch(5, then: t => t.Text("On")));

        var page = Assert.Single(created.Pages)!;
        // text + its line, the condition, the text and its line in the branch, the Else the editor needs, the end of the condition, the end of the list
        var commands = page.NamedCommands;
        Assert.Equal(0, commands[^1].Code);
        Assert.Equal(0, commands[^1].Indent);
        Assert.Contains(commands, c => c.Code == 111);
        Assert.Equal(2, commands.Count(c => c.Code == 101));
        Assert.Equal(commands.Count(c => c.Code == 111), commands.Count(c => c.Code == 412));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AFurtherPageCanBeMadeByTheStorage(Version v)
    {
        var storage = Open(v, Project(v));
        var created = NewMap(storage).CreateEvent(0, 0);

        var second = storage.New<IEventPage>();
        created.Pages.Add(second);
        second.Append(b => b.Text("Second"));

        Assert.Equal(2, created.Pages.Count);
        Assert.Contains(second.NamedCommands, c => c.Code == 101);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ClearEventsRemovesThemAllAndNumberingStartsAgain(Version v)
    {
        var project = Project(v);
        var storage = Open(v, project);
        var map = storage.CreateMap("Cave", 10, 8).Map;
        map.CreateEvent(0, 0);
        map.CreateEvent(1, 1);
        map.Events[9] = null;

        Assert.Equal(2, map.ClearEvents());
        Assert.Empty(map.Events);
        Assert.Equal(0, map.ClearEvents());
        Assert.Equal(1, map.CreateEvent(0, 0).Id);

        map.ClearEvents();
        storage.SaveAll();
        var loaded = Open(v, project).Maps.Get(3)!;
        Assert.DoesNotContain(loaded.Events.Values, e => e != null);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ASquareOutsideOfTheMapOrWithAnEventIsRefused(Version v)
    {
        var map = NewMap(Open(v, Project(v)));
        map.CreateEvent(3, 3);

        Assert.Throws<ArgumentOutOfRangeException>(() => map.CreateEvent(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.CreateEvent(10, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.CreateEvent(0, 8));
        Assert.Throws<InvalidOperationException>(() => map.CreateEvent(3, 3));
        Assert.Single(map.Events);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ANewEventIsSavedAndLoadedWithItsCommands(Version v)
    {
        var project = Project(v);
        var storage = Open(v, project);
        var map = storage.CreateMap("Cave", 10, 8).Map;
        map.CreateEvent(1, 1);
        map.CreateEvent(4, 5, "Talker", b => b.Text("Hi", "there"));

        storage.SaveAll();
        var again = Open(v, project);

        var loaded = again.Maps.Get(3)!.Events;
        Assert.Equal([1, 2], loaded.Where(p => p.Value != null).Select(p => p.Key).Order());
        var talker = loaded[2]!;
        Assert.Equal(("Talker", 4, 5, 2), (talker.Name, talker.X, talker.Y, talker.Id));
        Assert.Equal(map.Events[2]!.Pages[0]!.NamedCommands.Select(c => c.Code), talker.Pages[0]!.NamedCommands.Select(c => c.Code));
    }
}
