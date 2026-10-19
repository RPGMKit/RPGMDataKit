using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.VXAce.Classes;
using RPGMDataKit.Storages;
using RPGMDataKit.Storages.Abstraction;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// The tree of maps: the children of a map, putting a map under another one, opening and closing a branch. The project is the empty project of each editor (the maps 1 and 2, the map 2
/// under the map 1); the maps made here are 3 (under 2), 4 (root), 5 (under 1), 6 (under 3), so the tree is
/// <c>1 { 2 { 3 { 6 } }, 5 }, 4</c>.
/// </summary>
public sealed class MapTreeTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "RPGMDataKit.Tests." + Guid.NewGuid().ToString("N"));

    public MapTreeTests() => Directory.CreateDirectory(_folder);

    public void Dispose() => Directory.Delete(_folder, true);

    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private Storage Tree(Version v)
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

        Storage storage = v == Version.VXAce ? new VXAceStorage(project, "Data") : new MVStorage(project, "data");
        storage.CreateMap("three", parentId: 2);
        storage.CreateMap("four");
        storage.CreateMap("five", parentId: 1);
        storage.CreateMap("six", parentId: 3);

        return storage;
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheChildrenOfAMapAreInTheOrderOfTheTree(Version v)
    {
        var storage = Tree(v);

        Assert.Equal([1, 4], storage.GetChildMaps(0));
        Assert.Equal([2, 5], storage.GetChildMaps(1));
        Assert.Equal([3], storage.GetChildMaps(2));
        Assert.Equal([6], storage.GetChildMaps(3));
        Assert.Empty(storage.GetChildMaps(4));
        Assert.Throws<ArgumentException>(() => storage.GetChildMaps(99));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AMapIsPutUnderAnotherAfterTheMapsThereAndTakesItsChildrenWithIt(Version v)
    {
        var storage = Tree(v);

        storage.SetMapParent(3, 4);   // 3 with its child 6 goes under 4

        Assert.Equal(4, storage.MapInfos.Get(3)!.Parent);
        Assert.Equal([3], storage.GetChildMaps(4));
        Assert.Equal([6], storage.GetChildMaps(3));
        Assert.Empty(storage.GetChildMaps(2));

        storage.SetMapParent(2, 4);   // after the 3, whatever the old order was
        Assert.Equal([3, 2], storage.GetChildMaps(4));
        Assert.True(storage.MapInfos.Get(2)!.Order > storage.MapInfos.Get(3)!.Order);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AMapCanBeMovedToTheRoot(Version v)
    {
        var storage = Tree(v);

        storage.SetMapParent(5, 0);

        Assert.Equal(0, storage.MapInfos.Get(5)!.Parent);
        Assert.Equal([1, 4, 5], storage.GetChildMaps(0));
        Assert.Equal([2], storage.GetChildMaps(1));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AMapUnderItsParentAlreadyStaysWhereItIs(Version v)
    {
        var storage = Tree(v);
        var order = storage.MapInfos.Get(2)!.Order;

        storage.SetMapParent(2, 1);

        Assert.Equal(order, storage.MapInfos.Get(2)!.Order);
        Assert.Equal([2, 5], storage.GetChildMaps(1));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AMapCannotBePutUnderItselfOrUnderOneOfItsOwnChildrenAndNothingChanges(Version v)
    {
        var storage = Tree(v);

        Assert.Throws<ArgumentException>(() => storage.SetMapParent(2, 2));
        Assert.Throws<ArgumentException>(() => storage.SetMapParent(1, 6));   // 6 is under 3, under 2, under 1
        Assert.Throws<ArgumentException>(() => storage.SetMapParent(2, 3));
        Assert.Throws<ArgumentException>(() => storage.SetMapParent(99, 1));
        Assert.Throws<ArgumentException>(() => storage.SetMapParent(4, 99));

        Assert.Equal([1, 4], storage.GetChildMaps(0));
        Assert.Equal([2, 5], storage.GetChildMaps(1));
        Assert.Equal(0, storage.MapInfos.Get(1)!.Parent);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AMapMovedIsSavedUnderItsNewParent(Version v)
    {
        var storage = Tree(v);
        storage.SetMapParent(6, 4);
        storage.SaveAll();

        Storage again = v == Version.VXAce ? new VXAceStorage(storage.ProjectPath, "Data") : new MVStorage(storage.ProjectPath, "data");

        Assert.Equal([6], again.GetChildMaps(4));
        Assert.Empty(again.GetChildMaps(3));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AMapIsPutAtAPlaceAmongTheMapsOfItsLevel(Version v)
    {
        var storage = Tree(v);
        storage.CreateMap("seven", parentId: 1);   // 1 { 2, 5, 7 }
        Assert.Equal([2, 5, 7], storage.GetChildMaps(1));

        storage.ReorderMap(7, 0);
        Assert.Equal([7, 2, 5], storage.GetChildMaps(1));

        storage.ReorderMap(7, 2);
        Assert.Equal([2, 5, 7], storage.GetChildMaps(1));

        storage.ReorderMap(2, 1);
        Assert.Equal([5, 2, 7], storage.GetChildMaps(1));

        storage.ReorderMap(2, 1);   // already there
        Assert.Equal([5, 2, 7], storage.GetChildMaps(1));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheLevelAndThePlaceOfAMapAreKnown(Version v)
    {
        var storage = Tree(v);
        storage.CreateMap("seven", parentId: 1);   // 1 { 2, 5, 7 }

        Assert.Equal([2, 5, 7], storage.GetMapsAtLevel(5));
        Assert.Equal([2, 5, 7], storage.GetMapsAtLevel(2));
        Assert.Equal([1, 4], storage.GetMapsAtLevel(4));      // the root maps
        Assert.Equal([6], storage.GetMapsAtLevel(6));
        Assert.Equal(new MapTreeExtensions.MapPosition(1, 3), storage.GetMapPositionAtLevel(5));
        Assert.Equal(new MapTreeExtensions.MapPosition(0, 1), storage.GetMapPositionAtLevel(6));
        Assert.Throws<ArgumentException>(() => storage.GetMapsAtLevel(99));
        Assert.Throws<ArgumentException>(() => storage.GetMapPositionAtLevel(99));

        // every place from 0 to Count - 1 is one the map can be put at, and it is where it goes
        var count = storage.GetMapPositionAtLevel(2).Count;
        for (var place = 0; place < count; place++)
        {
            storage.ReorderMap(2, place);
            Assert.Equal(place, storage.GetMapPositionAtLevel(2).Index);
        }
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ThePlacesFromTheEndAreGivenWithTheCaret(Version v)
    {
        var storage = Tree(v);
        storage.CreateMap("seven", parentId: 1);   // 1 { 2, 5, 7 }

        storage.ReorderMap(2, ^1);
        Assert.Equal([5, 7, 2], storage.GetChildMaps(1));

        storage.ReorderMap(2, ^2);
        Assert.Equal([5, 2, 7], storage.GetChildMaps(1));

        storage.ReorderMap(7, ^3);   // the first
        Assert.Equal([7, 5, 2], storage.GetChildMaps(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => storage.ReorderMap(7, ^4));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ReorderingExchangesTheNumbersOfTheLevelAndTouchesNothingElse(Version v)
    {
        var storage = Tree(v);
        storage.CreateMap("seven", parentId: 1);
        int Order(int id) => storage.MapInfos.Get(id)!.Order;
        var before = new[] { 1, 2, 3, 4, 5, 6, 7 }.ToDictionary(id => id, Order);

        storage.ReorderMap(7, 0);

        // the maps 2, 5 and 7 have the numbers 2, 5 and 7 between them, in the new order
        Assert.Equal([before[2], before[5], before[7]], new[] { Order(7), Order(2), Order(5) });
        foreach (var id in new[] { 1, 3, 4, 6 })
            Assert.Equal(before[id], Order(id));

        Assert.Equal([3], storage.GetChildMaps(2));   // the children of a map that moved are where they were
        Assert.All(new[] { 2, 5, 7 }, id => Assert.Equal(1, storage.MapInfos.Get(id)!.Parent));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheMapsOfTheRootAndAnOnlyChildCanBeReorderedToo(Version v)
    {
        var storage = Tree(v);

        storage.ReorderMap(4, 0);
        Assert.Equal([4, 1], storage.GetChildMaps(0));

        storage.ReorderMap(6, 0);   // the only one of its level
        Assert.Equal([6], storage.GetChildMaps(3));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ALevelWhoseNumbersAreTheSameGetsNewOnes(Version v)
    {
        var storage = Tree(v);
        storage.MapInfos.Get(5)!.Order = storage.MapInfos.Get(2)!.Order;   // as a file that was not made here can have

        storage.ReorderMap(5, 0);

        Assert.Equal([5, 2], storage.GetChildMaps(1));
        Assert.NotEqual(storage.MapInfos.Get(2)!.Order, storage.MapInfos.Get(5)!.Order);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void APlaceOutsideOfTheLevelOrAnUnknownMapIsRefused(Version v)
    {
        var storage = Tree(v);

        Assert.Throws<ArgumentOutOfRangeException>(() => storage.ReorderMap(2, 2));   // the level of 2 has two maps: 2 and 5
        Assert.Throws<ArgumentOutOfRangeException>(() => storage.ReorderMap(2, -1));
        Assert.Throws<ArgumentException>(() => storage.ReorderMap(99, 0));
        Assert.Equal([2, 5], storage.GetChildMaps(1));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheNewOrderIsSavedAndLoaded(Version v)
    {
        var storage = Tree(v);
        storage.ReorderMap(5, 0);
        storage.SaveAll();

        Storage again = v == Version.VXAce ? new VXAceStorage(storage.ProjectPath, "Data") : new MVStorage(storage.ProjectPath, "data");

        Assert.Equal([5, 2], again.GetChildMaps(1));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AMapIsOpenedAndClosedAlone(Version v)
    {
        var storage = Tree(v);
        storage.CollapseMap(1);
        Assert.False(storage.MapInfos.Get(1)!.Expanded);

        storage.ExpandMap(2);

        Assert.True(storage.MapInfos.Get(2)!.Expanded);
        Assert.False(storage.MapInfos.Get(1)!.Expanded);   // only the map that was asked
        Assert.False(storage.MapInfos.Get(3)!.Expanded);

        storage.CollapseMap(2);
        Assert.False(storage.MapInfos.Get(2)!.Expanded);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ABranchIsOpenedAndClosedWithEverythingUnderIt(Version v)
    {
        var storage = Tree(v);
        storage.CollapseMap(1, recursive: true);
        Assert.All(new[] { 1, 2, 3, 5, 6 }, id => Assert.False(storage.MapInfos.Get(id)!.Expanded));

        storage.ExpandMap(2, recursive: true);

        Assert.True(storage.MapInfos.Get(2)!.Expanded);
        Assert.True(storage.MapInfos.Get(3)!.Expanded);
        Assert.True(storage.MapInfos.Get(6)!.Expanded);
        Assert.False(storage.MapInfos.Get(1)!.Expanded);   // above it
        Assert.False(storage.MapInfos.Get(5)!.Expanded);   // beside it
        Assert.False(storage.MapInfos.Get(4)!.Expanded);

        storage.CollapseMap(2, recursive: true);
        Assert.False(storage.MapInfos.Get(6)!.Expanded);
        Assert.Throws<ArgumentException>(() => storage.ExpandMap(99));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void MovingAMapDoesNotOpenOrCloseAnything(Version v)
    {
        var storage = Tree(v);
        storage.CollapseMap(4);

        storage.SetMapParent(3, 4);

        Assert.False(storage.MapInfos.Get(4)!.Expanded);
    }
}
