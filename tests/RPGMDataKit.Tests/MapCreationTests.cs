using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.VXAce.Classes;
using RPGMDataKit.Storages;
using RPGMDataKit.Storages.Abstraction;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// <see cref="MapCreationExtensions.CreateMap"/>: a new map and its entry in the list of maps, made as the editors make them. The project is the data of an empty project of each editor
/// (<c>Projects</c>), which has the maps 1 and 2; the editors gave the map 2 the number 2, the order 2, the size 17 x 13 and the first tileset, and the view of the editor
/// in the middle of the map (the scroll of VX Ace is 272, 208 = half of 17 x 13 tiles of 32 pixels).
/// </summary>
public sealed class MapCreationTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "RPGMDataKit.Tests." + Guid.NewGuid().ToString("N"));

    public MapCreationTests() => Directory.CreateDirectory(_folder);

    public void Dispose() => Directory.Delete(_folder, true);

    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private string Project(Version v, string name = "project")
    {
        var project = Path.Combine(_folder, name);
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

    // the list of MV has an empty slot 0
    private static List<int> ListedMaps(Storage storage) => storage.MapInfos.Get().Where(p => p.Value != null).Select(p => p.Key).Order().ToList();

    private static Storage Open(Version v, string project) => v == Version.VXAce ? new VXAceStorage(project, "Data") : new MVStorage(project, "data");

    [Theory]
    [MemberData(nameof(Versions))]
    public void ANewMapGetsTheNextNumberAndOrderAndTheValuesOfANewMap(Version v)
    {
        var storage = Open(v, Project(v));

        var created = storage.CreateMap("Cave", 30, 20, tilesetId: 1);

        Assert.Equal(3, created.Id);
        Assert.Same(created.Map, storage.Maps.Get(3));
        Assert.Same(created.Info, storage.MapInfos.Get(3));
        Assert.Equal(3, created.Info.Id);
        Assert.Equal(("Cave", 3, 0), (created.Info.Name, created.Info.Order, created.Info.Parent));
        Assert.False(created.Info.Expanded);

        Assert.Equal((30, 20, 1), (created.Map.Width, created.Map.Height, created.Map.TilesetId));
        Assert.Equal("", created.Map.DisplayName);
        Assert.Equal(30 * 20 * (v == Version.VXAce ? 4 : 6), created.Map.Data.Count);
        Assert.Equal((30, 20, v == Version.VXAce ? 4 : 6), (created.Map.Data.SizeX, created.Map.Data.SizeY, created.Map.Data.SizeZ));
        Assert.All(created.Map.Data.Elements, cell => Assert.Equal(0, cell));
        Assert.Empty(created.Map.Events);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheViewOfTheEditorStartsInTheMiddleOfTheMap(Version v)
    {
        var storage = Open(v, Project(v));

        var info = storage.CreateMap("Cave", 17, 13).Info;

        var tile = v == Version.VXAce ? 32 : 48;
        Assert.Equal((17 * tile / 2, 13 * tile / 2), (info.ScrollX, info.ScrollY));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheOtherValuesAreTheOnesOfANewMapOfTheMaker(Version v)
    {
        var storage = Open(v, Project(v));

        var made = storage.CreateMap("Cave").Map;
        var fresh = storage.New<IMap>();

        Assert.Equal((fresh.Width, fresh.Height, fresh.TilesetId), (made.Width, made.Height, made.TilesetId));
        Assert.Equal((fresh.ParallaxName, fresh.ParallaxLoopX, fresh.SpecifyBattleback, fresh.Note), (made.ParallaxName, made.ParallaxLoopX, made.SpecifyBattleback, made.Note));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AMapCanBeUnderAnother(Version v)
    {
        var storage = Open(v, Project(v));

        var child = storage.CreateMap("Room", parentId: 2);
        var next = storage.CreateMap("Hall");

        Assert.Equal(2, child.Info.Parent);
        Assert.Equal((3, 4), (child.Id, next.Id));
        Assert.Equal((3, 4), (child.Info.Order, next.Info.Order));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ANumberThatAMapFileHoldsWithoutAnEntryIsNotUsed(Version v)
    {
        var storage = Open(v, Project(v));
        storage.Maps.Set(3, storage.New<IMap>());

        var created = storage.CreateMap("Cave");

        Assert.Equal(4, created.Id);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void WrongArgumentsAreRefusedAndNothingIsAdded(Version v)
    {
        var storage = Open(v, Project(v));

        Assert.Throws<ArgumentOutOfRangeException>(() => storage.CreateMap("Cave", width: 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => storage.CreateMap("Cave", height: 0));
        Assert.Throws<ArgumentException>(() => storage.CreateMap("Cave", tilesetId: 99));
        Assert.Throws<ArgumentException>(() => storage.CreateMap("Cave", parentId: 99));
        Assert.Throws<ArgumentNullException>(() => storage.CreateMap(null!));

        Assert.Equal([1, 2], ListedMaps(storage));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ANewMapIsSavedAndLoadedWithItsTiles(Version v)
    {
        var project = Project(v);
        var storage = Open(v, project);
        var created = storage.CreateMap("Cave", 8, 6, parentId: 1);
        created.Map.SetTile(7, 5, 0, TileIds.MakeAutotile(16, 0));
        created.Map.SetRegion(7, 5, 3);

        storage.SaveAll();
        var again = Open(v, project);

        var map = again.Maps.Get(3)!;
        var info = again.MapInfos.Get(3)!;
        Assert.Equal((8, 6), (map.Width, map.Height));
        Assert.Equal(TileIds.MakeAutotile(16, 0), map.GetTile(7, 5, 0));
        Assert.Equal(3, map.GetRegion(7, 5));
        Assert.Equal(("Cave", 3, 1, 3), (info.Name, info.Id, info.Parent, info.Order));
        Assert.Equal([1, 2, 3], ListedMaps(again));
    }
}
