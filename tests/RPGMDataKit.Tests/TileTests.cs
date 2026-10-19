using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// The helpers for tiles: the meaning of a tile id (<see cref="TileIds"/>, the same functions as <c>Tilemap</c> of MV; the whole range 0-8191 was compared with the engine
/// once), the layers, shadows and regions of a map of either maker (<see cref="MapTileExtensions"/>) and the sheets and flags of a tileset (<see cref="TilesetExtensions"/>).
/// </summary>
public sealed class TileTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "RPGMDataKit.Tests." + Guid.NewGuid().ToString("N"));

    public TileTests() => Directory.CreateDirectory(_folder);

    public void Dispose() => Directory.Delete(_folder, true);

    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    // ---- tile ids ----

    [Theory]
    [InlineData(0, null)]
    [InlineData(1, TileSheet.B)]
    [InlineData(255, TileSheet.B)]
    [InlineData(256, TileSheet.C)]
    [InlineData(512, TileSheet.D)]
    [InlineData(1023, TileSheet.E)]
    [InlineData(1024, null)]
    [InlineData(1535, null)]
    [InlineData(1536, TileSheet.A5)]
    [InlineData(2047, TileSheet.A5)]
    [InlineData(2048, TileSheet.A1)]
    [InlineData(2815, TileSheet.A1)]
    [InlineData(2816, TileSheet.A2)]
    [InlineData(4352, TileSheet.A3)]
    [InlineData(5888, TileSheet.A4)]
    [InlineData(8191, TileSheet.A4)]
    [InlineData(8192, null)]
    [InlineData(-1, null)]
    public void TheSheetOfATileIsFoundByItsRange(int tileId, TileSheet? sheet)
    {
        Assert.Equal(sheet, TileIds.SheetOf(tileId));
    }

    [Fact]
    public void TheSheetIsThePlaceOfItsNameInTheTileset()
    {
        Assert.Equal([0, 1, 2, 3, 4, 5, 6, 7, 8], Enum.GetValues<TileSheet>().Select(s => (int)s).Order());
        Assert.Equal((int)TileSheet.A1, 0);
        Assert.Equal((int)TileSheet.E, 8);
    }

    [Fact]
    public void AnAutotileIsMadeOfItsKindAndShapeAndReadBack()
    {
        for (var kind = 0; kind < TileIds.AutotileKinds; kind++)
        {
            for (var shape = 0; shape < TileIds.ShapesPerKind; shape++)
            {
                var id = TileIds.MakeAutotile(kind, shape);
                Assert.True(TileIds.IsAutotile(id));
                Assert.Equal((kind, shape), (TileIds.Kind(id), TileIds.Shape(id)));
            }
        }

        Assert.Equal(TileIds.A1, TileIds.MakeAutotile(0, 0));
        Assert.Equal(TileIds.Max - 1, TileIds.MakeAutotile(127, 47));
        Assert.Equal(128, TileIds.AutotileKinds);
        Assert.Throws<ArgumentOutOfRangeException>(() => TileIds.MakeAutotile(128, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => TileIds.MakeAutotile(0, 48));
        Assert.Throws<ArgumentOutOfRangeException>(() => TileIds.MakeAutotile(-1, 0));
    }

    [Fact]
    public void TheKindsAreNumberedFromA1ThroughTheSheets()
    {
        Assert.Equal(0, TileIds.Kind(TileIds.A1));
        Assert.Equal(16, TileIds.Kind(TileIds.A2));
        Assert.Equal(48, TileIds.Kind(TileIds.A3));
        Assert.Equal(80, TileIds.Kind(TileIds.A4));
    }

    [Theory]
    [InlineData(1, 1, 0)]
    [InlineData(7, 7, 0)]
    [InlineData(8, 0, 1)]
    [InlineData(127, 7, 15)]
    [InlineData(128, 8, 0)]
    [InlineData(255, 15, 15)]
    [InlineData(256, 0, 0)]
    [InlineData(TileIds.A5, 0, 0)]
    [InlineData(TileIds.A5 + 127, 7, 15)]
    public void ANormalTileSitsInTheLeftHalfOfItsSheetForTheFirst128AndInTheRightForTheNext(int tileId, int column, int row)
    {
        Assert.Equal((column, row), TileIds.NormalTileCell(tileId));
    }

    [Fact]
    public void TheKindsOfTheWallsAndRoofsAreFoundByTheirPlaceInEachSixteen()
    {
        // A3: the kinds 48-55 are roofs, 56-63 the sides of houses, 64-71 roofs, 72-79 sides
        Assert.True(TileIds.IsRoof(TileIds.MakeAutotile(48, 0)));
        Assert.True(TileIds.IsWallSide(TileIds.MakeAutotile(56, 0)));
        Assert.True(TileIds.IsRoof(TileIds.MakeAutotile(64, 0)));
        Assert.True(TileIds.IsWallSide(TileIds.MakeAutotile(79, 0)));

        // A4: the kinds 80-87 are the tops of walls, 88-95 their sides
        Assert.True(TileIds.IsWallTop(TileIds.MakeAutotile(80, 0)));
        Assert.True(TileIds.IsWallSide(TileIds.MakeAutotile(88, 0)));
        Assert.True(TileIds.IsWall(TileIds.MakeAutotile(127, 0)));

        Assert.True(TileIds.IsWallTypeAutotile(TileIds.MakeAutotile(56, 5)));
        Assert.True(TileIds.IsFloorTypeAutotile(TileIds.MakeAutotile(80, 5)));
        Assert.True(TileIds.IsFloorTypeAutotile(TileIds.MakeAutotile(16, 5)));
    }

    [Fact]
    public void WaterAndWaterfallsAreTheKindsOfA1()
    {
        Assert.True(TileIds.IsWater(TileIds.MakeAutotile(0, 0)));
        Assert.True(TileIds.IsWater(TileIds.MakeAutotile(1, 0)));
        Assert.False(TileIds.IsWater(TileIds.MakeAutotile(2, 0)));   // the static kinds
        Assert.False(TileIds.IsWater(TileIds.MakeAutotile(3, 0)));
        Assert.True(TileIds.IsWater(TileIds.MakeAutotile(4, 0)));

        Assert.False(TileIds.IsWaterfall(TileIds.MakeAutotile(4, 0)));
        Assert.True(TileIds.IsWaterfall(TileIds.MakeAutotile(5, 0)));
        Assert.True(TileIds.IsWaterfall(TileIds.MakeAutotile(15, 3)));
        Assert.False(TileIds.IsFloorTypeAutotile(TileIds.MakeAutotile(5, 0)));
        Assert.True(TileIds.IsWaterfallTypeAutotile(TileIds.MakeAutotile(5, 0)));
    }

    // ---- the layers, shadows and regions of a map ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void AMapHasTheLayersOfItsMaker(Version v)
    {
        var map = v.New<IMap>();
        Assert.Equal(v == Version.VXAce ? 3 : 4, map.TileLayerCount());
        Assert.Equal(v == Version.VXAce ? 4 : 6, map.Data.SizeZ);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ATileIsPutOnALayerAndReadBack(Version v)
    {
        var map = v.New<IMap>();
        var top = map.TileLayerCount() - 1;

        map.SetTile(2, 3, 0, TileIds.MakeAutotile(16, 0));
        map.SetTile(2, 3, top, 25);

        Assert.Equal(TileIds.MakeAutotile(16, 0), map.GetTile(2, 3, 0));
        Assert.Equal(25, map.GetTile(2, 3, top));
        Assert.Equal(0, map.GetTile(2, 3, 1));
        Assert.Equal(0, map.GetTile(3, 3, 0));

        // the cell is where the engines look for it: x first, then y, then the layer
        Assert.Equal(25, map.Data.Elements[2 + 3 * map.Width + top * map.Width * map.Height]);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void LayeredTilesAreTheTilesFromTheTopLayerDown(Version v)
    {
        var map = v.New<IMap>();
        var count = map.TileLayerCount();
        for (var layer = 0; layer < count; layer++)
            map.SetTile(1, 1, layer, 10 + layer);

        var tiles = map.LayeredTiles(1, 1);

        Assert.Equal(Enumerable.Range(0, count).Select(i => 10 + count - 1 - i), tiles);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ASquareLayerOrTileOutsideOfTheMapIsRefused(Version v)
    {
        var map = v.New<IMap>();
        var layers = map.TileLayerCount();

        Assert.Throws<ArgumentOutOfRangeException>(() => map.GetTile(map.Width, 0, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.GetTile(0, map.Height, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.GetTile(-1, 0, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.GetTile(0, 0, layers));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.SetTile(0, 0, 0, TileIds.Max));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.SetTile(0, 0, 0, -1));
    }

    [Fact]
    public void InAVxAceMapTheShadowsAndTheRegionShareTheLastLayer()
    {
        var map = Version.VXAce.New<IMap>();

        map.SetRegion(4, 5, 7);
        map.SetShadow(4, 5, 9);

        Assert.Equal((7, 9), (map.GetRegion(4, 5), map.GetShadow(4, 5)));
        Assert.Equal((7 << 8) | 9, map.Data[4, 5, 3]);

        map.SetRegion(4, 5, 0);
        Assert.Equal(9, map.Data[4, 5, 3]);   // the shadows stay

        map.SetShadow(4, 5, 0);
        Assert.Equal(0, map.Data[4, 5, 3]);
    }

    [Fact]
    public void InAnMvMapTheShadowsAndTheRegionHaveALayerEach()
    {
        var map = Version.Mv.New<IMap>();

        map.SetRegion(4, 5, 200);
        map.SetShadow(4, 5, 9);

        Assert.Equal((200, 9), (map.GetRegion(4, 5), map.GetShadow(4, 5)));
        Assert.Equal(9, map.Data[4, 5, 4]);
        Assert.Equal(200, map.Data[4, 5, 5]);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AShadowIsFourBitsAndARegionIsNotNegative(Version v)
    {
        var map = v.New<IMap>();

        map.SetShadow(0, 0, 15);
        Assert.Equal(15, map.GetShadow(0, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.SetShadow(0, 0, 16));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.SetShadow(0, 0, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.SetRegion(0, 0, -1));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AShadowCanBeGivenAsTheDarkQuarters(Version v)
    {
        var map = v.New<IMap>();

        map.SetShadow(1, 1, ShadowFlag.TopLeft | ShadowFlag.BottomRight);

        // the quarters are the bits 0-3: top left, top right, bottom left, bottom right
        Assert.Equal(1 | 8, map.GetShadow(1, 1));
        Assert.Equal(ShadowFlag.TopLeft | ShadowFlag.BottomRight, map.GetShadowFlags(1, 1));
        Assert.Equal("TopLeft, BottomRight", map.GetShadowFlags(1, 1).ToString());

        map.SetShadow(1, 1, ShadowFlag.All);
        Assert.Equal(15, map.GetShadow(1, 1));

        map.SetShadow(1, 1, ShadowFlag.None);
        Assert.Equal(ShadowFlag.None, map.GetShadowFlags(1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.SetShadow(1, 1, (ShadowFlag)16));

        // a number still works, and a plain 0 is the number
        map.SetShadow(1, 1, 6);
        Assert.Equal(ShadowFlag.TopRight | ShadowFlag.BottomLeft, map.GetShadowFlags(1, 1));
        map.SetShadow(1, 1, 0);
        Assert.Equal(0, map.GetShadow(1, 1));
    }

    [Fact]
    public void ARegionOfAVxAceMapMustFitTheSigned16BitCell()
    {
        var map = Version.VXAce.New<IMap>();

        map.SetRegion(0, 0, MapTileExtensions.MaxRegionVxAce);
        Assert.Equal(MapTileExtensions.MaxRegionVxAce, map.GetRegion(0, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.SetRegion(0, 0, MapTileExtensions.MaxRegionVxAce + 1));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TilesShadowsAndRegionsSurviveSavingAndLoading(Version v)
    {
        var map = v.New<IMap>();
        map.SetTile(3, 2, 0, TileIds.MakeAutotile(20, 10));
        map.SetTile(3, 2, map.TileLayerCount() - 1, 99);
        map.SetShadow(3, 2, 5);
        map.SetRegion(3, 2, 12);
        var path = Path.Combine(_folder, "Map001" + v.Serializer.FileExtension);

        v.Save(map, path);
        var loaded = (IMap)v.Load(typeof(IMap), path);

        Assert.Equal(TileIds.MakeAutotile(20, 10), loaded.GetTile(3, 2, 0));
        Assert.Equal(99, loaded.GetTile(3, 2, loaded.TileLayerCount() - 1));
        Assert.Equal((5, 12), (loaded.GetShadow(3, 2), loaded.GetRegion(3, 2)));
    }

    // ---- a tileset ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void ASheetNameIsFoundByTheSheetAndByATileOfIt(Version v)
    {
        var tileset = v.New<ITileSet>();

        Assert.Equal("", tileset.SheetName(TileSheet.A2));
        tileset.SetSheetName(TileSheet.A2, "Outside_A2");
        tileset.SetSheetName(TileSheet.B, "Outside_B");

        Assert.Equal("Outside_A2", tileset.SheetName(TileSheet.A2));
        Assert.Equal("Outside_A2", tileset.SheetNameOf(TileIds.MakeAutotile(16, 0)));
        Assert.Equal("Outside_B", tileset.SheetNameOf(5));
        Assert.Equal("", tileset.SheetNameOf(0));      // nothing
        Assert.Equal("", tileset.SheetNameOf(1100));   // an unused id
        Assert.Equal(9, tileset.TilesetNames.Count);
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheFlagsOfATileAreReadAndWrittenAndTheTableGrowsWhenItIsShort(Version v)
    {
        var tileset = v.New<ITileSet>();
        tileset.Flags.Elements.RemoveRange(1, tileset.Flags.Count - 1);   // as short as the one the editor of MV makes
        tileset.Flags.Reshape(tileset.Flags.Count);

        Assert.Equal(0, tileset.GetFlags(100));   // past the end: none
        tileset.SetFlags(100, (int)(TileFlags.Ladder | TileFlags.PassNoUp));

        Assert.Equal(101, tileset.Flags.Count);
        Assert.Equal((int)(TileFlags.Ladder | TileFlags.PassNoUp), tileset.GetFlags(100));
        Assert.Equal(0, tileset.GetFlags(99));
        Assert.True(tileset.HasFlags(100, TileFlags.Ladder));
        Assert.True(tileset.HasFlags(100, TileFlags.Ladder | TileFlags.PassNoUp));
        Assert.False(tileset.HasFlags(100, TileFlags.Bush));
        Assert.Throws<ArgumentOutOfRangeException>(() => tileset.SetFlags(TileIds.Max, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => tileset.SetFlags(1, 0x8000));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheTerrainTagIsTheUpperBitsAndTheOtherFlagsStay(Version v)
    {
        var tileset = v.New<ITileSet>();
        tileset.SetFlags(7, (int)TileFlags.Bush);

        tileset.SetTerrainTag(7, 5);

        Assert.Equal(5, tileset.GetTerrainTag(7));
        Assert.True(tileset.HasFlags(7, TileFlags.Bush));
        Assert.Equal(0x5040, tileset.GetFlags(7));

        tileset.SetTerrainTag(7, 0);
        Assert.Equal((int)TileFlags.Bush, tileset.GetFlags(7));
        Assert.Throws<ArgumentOutOfRangeException>(() => tileset.SetTerrainTag(7, 8));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ADirectionIsBlockedByItsBit(Version v)
    {
        var tileset = v.New<ITileSet>();
        tileset.SetFlags(9, (int)(TileFlags.PassNoDown | TileFlags.PassNoRight));

        Assert.True(tileset.BlocksDirection(9, Direction.Down));
        Assert.True(tileset.BlocksDirection(9, Direction.Right));
        Assert.False(tileset.BlocksDirection(9, Direction.Left));
        Assert.False(tileset.BlocksDirection(9, Direction.Up));
        Assert.Throws<ArgumentOutOfRangeException>(() => tileset.BlocksDirection(9, Direction.Retain));
    }
}
