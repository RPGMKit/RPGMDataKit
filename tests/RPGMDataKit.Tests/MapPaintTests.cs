using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// Painting on the layer the editors choose (<see cref="MapPaintExtensions.PaintTileAuto"/>). The MV cases are the ones that were painted in the editor of MV (1.6.x, layer on Auto) in the order written
/// in each test, and their saved layers; the VX Ace ones follow the layers of a saved game.
/// </summary>
public sealed class MapPaintTests
{
    private const int Grass = 16;      // A2, column 0: the ground
    private const int Bush = 20;       // A2, column 4: an overlay
    private const int Overlay2 = 22;   // A2, column 6: another overlay
    private const int WallSide = 120;  // A4
    private const int WallSide2 = 121;
    private const int Roof = 48;       // A3
    private const int A5Tile = 1553;

    private static int Kind(int kind) => TileIds.MakeAutotile(kind, 0);

    private static int[] Layers(IMap map, int x = 0, int y = 0) =>
        Enumerable.Range(0, map.TileLayerCount()).Select(z => map.GetTile(x, y, z)).Select(t => TileIds.IsAutotile(t) ? TileIds.Kind(t) + 10000 : t).ToArray();

    // an autotile is shown as 10000 + its kind, the tile of B-E and A5 as its id, so the expectations read as "what is on which layer"
    private static int K(int kind) => 10000 + kind;

    private static IMap Mv() => Version.Mv.New<IMap>();

    private static IMap Vx() => Version.VXAce.New<IMap>();

    // ---- the kinds that are overlays ----

    [Fact]
    public void TheRightHalfOfA2AndTheDeepSeaAreOverlaysAndTheRestIsGround()
    {
        for (var kind = 16; kind < 48; kind++)
            Assert.Equal((kind - 16) % 8 >= 4, TileIds.IsOverlay(Kind(kind)));

        Assert.True(TileIds.IsOverlay(Kind(1)));
        Assert.False(TileIds.IsOverlay(Kind(0)));
        Assert.False(TileIds.IsOverlay(Kind(4)));
        Assert.False(TileIds.IsOverlay(Kind(48)));
        Assert.False(TileIds.IsOverlay(Kind(80)));
        Assert.False(TileIds.IsOverlay(A5Tile));
        Assert.False(TileIds.IsOverlay(24));
        Assert.False(TileIds.IsOverlay(0));
    }

    // ---- MV ----

    [Fact]
    public void MvGroundIsPaintedOnLayer0AndAnotherGroundTakesItsPlace()
    {
        var map = Mv();
        map.PaintTileAuto(0, 0, Kind(Grass));
        map.PaintTileAuto(1, 0, Kind(Grass));
        map.PaintTileAuto(1, 0, Kind(Grass));
        map.PaintTileAuto(2, 0, Kind(17));
        map.PaintTileAuto(2, 0, Kind(18));

        Assert.Equal([K(Grass), 0, 0, 0], Layers(map, 0));
        Assert.Equal([K(Grass), 0, 0, 0], Layers(map, 1));
        Assert.Equal([K(18), 0, 0, 0], Layers(map, 2));
    }

    [Fact]
    public void MvAnOverlayIsPaintedOnLayer1EvenWithNothingUnderItAndTakesThePlaceOfAnotherOverlay()
    {
        var map = Mv();
        map.PaintTileAuto(0, 0, Kind(Bush));
        map.PaintTileAuto(1, 0, Kind(Bush));
        map.PaintTileAuto(1, 0, Kind(Bush));
        map.PaintTileAuto(2, 0, Kind(Bush));
        map.PaintTileAuto(2, 0, Kind(Overlay2));

        Assert.Equal([0, K(Bush), 0, 0], Layers(map, 0));
        Assert.Equal([0, K(Bush), 0, 0], Layers(map, 1));
        Assert.Equal([0, K(Overlay2), 0, 0], Layers(map, 2));
    }

    [Fact]
    public void MvAnOverlayStaysOverTheGroundAndTheGroundPaintedOverAnOverlayRemovesIt()
    {
        var map = Mv();
        map.PaintTileAuto(0, 0, Kind(Grass));
        map.PaintTileAuto(0, 0, Kind(Bush));
        map.PaintTileAuto(1, 0, Kind(Bush));
        map.PaintTileAuto(1, 0, Kind(Grass));

        Assert.Equal([K(Grass), K(Bush), 0, 0], Layers(map, 0));
        Assert.Equal([K(Grass), 0, 0, 0], Layers(map, 1));
    }

    [Fact]
    public void MvATileOfBToEGoesToTheTopLayerWhateverIsUnderIt()
    {
        var map = Mv();
        map.PaintTileAuto(0, 0, 88);
        map.PaintTileAuto(1, 0, Kind(Grass));
        map.PaintTileAuto(1, 0, 88);
        map.PaintTileAuto(2, 0, Kind(Grass));
        map.PaintTileAuto(2, 0, Kind(Bush));
        map.PaintTileAuto(2, 0, 88);

        Assert.Equal([0, 0, 0, 88], Layers(map, 0));
        Assert.Equal([K(Grass), 0, 0, 88], Layers(map, 1));
        Assert.Equal([K(Grass), K(Bush), 0, 88], Layers(map, 2));
    }

    [Fact]
    public void MvAnotherTileOfBToEMovesTheOneOnTopDownAndTheOneBelowIsLost()
    {
        var map = Mv();
        map.PaintTileAuto(0, 0, Kind(Grass));
        map.PaintTileAuto(0, 0, 88);
        map.PaintTileAuto(0, 0, 89);
        map.PaintTileAuto(1, 0, Kind(Grass));
        map.PaintTileAuto(1, 0, 88);
        map.PaintTileAuto(1, 0, 89);
        map.PaintTileAuto(1, 0, 90);

        Assert.Equal([K(Grass), 0, 88, 89], Layers(map, 0));
        Assert.Equal([K(Grass), 0, 89, 90], Layers(map, 1));
    }

    [Fact]
    public void MvAWallARoofOrA5TakeThePlaceOfTheGroundOnLayer0()
    {
        var map = Mv();
        map.PaintTileAuto(0, 0, Kind(Grass));
        map.PaintTileAuto(0, 0, Kind(WallSide));
        map.PaintTileAuto(1, 0, Kind(WallSide));
        map.PaintTileAuto(1, 0, Kind(WallSide2));
        map.PaintTileAuto(2, 0, Kind(Grass));
        map.PaintTileAuto(2, 0, Kind(Roof));
        map.PaintTileAuto(3, 0, Kind(Grass));
        map.PaintTileAuto(3, 0, A5Tile);
        map.PaintTileAuto(3, 0, 24);

        Assert.Equal([K(WallSide), 0, 0, 0], Layers(map, 0));
        Assert.Equal([K(WallSide2), 0, 0, 0], Layers(map, 1));
        Assert.Equal([K(Roof), 0, 0, 0], Layers(map, 2));
        Assert.Equal([A5Tile, 0, 0, 24], Layers(map, 3));
    }

    [Fact]
    public void MvGroundPaintedOverASquareRemovesEverythingElseOnIt()
    {
        // painted in the editor: the sand (kind 24) over grass and a B tile; over grass, a bush and a B tile; over grass and two B tiles; B tiles alone and then grass
        var map = Mv();
        map.PaintTileAuto(0, 0, Kind(Grass)); map.PaintTileAuto(0, 0, 88); map.PaintTileAuto(0, 0, Kind(24));
        map.PaintTileAuto(1, 0, Kind(Grass)); map.PaintTileAuto(1, 0, Kind(Bush)); map.PaintTileAuto(1, 0, 88); map.PaintTileAuto(1, 0, Kind(24));
        map.PaintTileAuto(2, 0, Kind(Grass)); map.PaintTileAuto(2, 0, 88); map.PaintTileAuto(2, 0, 89); map.PaintTileAuto(2, 0, Kind(24));
        map.PaintTileAuto(3, 0, 88); map.PaintTileAuto(3, 0, 89); map.PaintTileAuto(3, 0, Kind(Grass));

        Assert.Equal([K(24), 0, 0, 0], Layers(map, 0));
        Assert.Equal([K(24), 0, 0, 0], Layers(map, 1));
        Assert.Equal([K(24), 0, 0, 0], Layers(map, 2));
        Assert.Equal([K(Grass), 0, 0, 0], Layers(map, 3));
    }

    [Fact]
    public void MvAWallARoofOrA5PaintedOverAFullSquareRemovesEverythingElseOnIt()
    {
        var map = Mv();
        foreach (var (x, tile) in new[] { (0, Kind(WallSide)), (1, Kind(Roof)), (2, A5Tile) })
        {
            map.PaintTileAuto(x, 0, Kind(Grass));
            map.PaintTileAuto(x, 0, Kind(Bush));
            map.PaintTileAuto(x, 0, 88);
            map.PaintTileAuto(x, 0, tile);
        }

        Assert.Equal([K(WallSide), 0, 0, 0], Layers(map, 0));
        Assert.Equal([K(Roof), 0, 0, 0], Layers(map, 1));
        Assert.Equal([A5Tile, 0, 0, 0], Layers(map, 2));
    }

    [Fact]
    public void MvTheSameTileOfBToEPaintedAgainIsNotAddedAgain()
    {
        var map = Mv();
        map.PaintTileAuto(0, 0, Kind(Grass));
        map.PaintTileAuto(0, 0, 88);
        map.PaintTileAuto(0, 0, 88);

        Assert.Equal([K(Grass), 0, 0, 88], Layers(map, 0));
    }

    [Fact]
    public void MvB1ThenB2ThenB2AgainIsB1UnderB2()
    {
        var map = Mv();
        map.PaintTileAuto(0, 0, 88);
        map.PaintTileAuto(0, 0, 89);
        map.PaintTileAuto(0, 0, 89);

        Assert.Equal([0, 0, 88, 89], Layers(map));
    }

    [Fact]
    public void MvB1ThenB2ThenB1AgainPutsB2UnderB1()
    {
        var map = Mv();
        map.PaintTileAuto(0, 0, 88);
        map.PaintTileAuto(0, 0, 89);
        map.PaintTileAuto(0, 0, 88);

        Assert.Equal([0, 0, 89, 88], Layers(map));
    }

    [Fact]
    public void MvAnOverlayPaintedOverTilesOfBToERemovesThemAndKeepsTheGround()
    {
        // painted in the editor: grass, B1, B2, bush; and grass, bush, B1, another overlay
        var map = Mv();
        map.PaintTileAuto(0, 0, Kind(Grass));
        map.PaintTileAuto(0, 0, 88);
        map.PaintTileAuto(0, 0, 95);
        map.PaintTileAuto(0, 0, Kind(Bush));
        map.PaintTileAuto(1, 0, Kind(Grass));
        map.PaintTileAuto(1, 0, Kind(Bush));
        map.PaintTileAuto(1, 0, 88);
        map.PaintTileAuto(1, 0, Kind(Overlay2));

        Assert.Equal([K(Grass), K(Bush), 0, 0], Layers(map, 0));
        Assert.Equal([K(Grass), K(Overlay2), 0, 0], Layers(map, 1));
    }

    [Fact]
    public void MvTheTilesRoundTheSquareAreReshapedOnEveryLayer()
    {
        var map = Mv();
        map.PaintTileAuto(5, 5, Kind(Grass));
        map.PaintTileAuto(5, 5, Kind(Bush));
        map.PaintTileAuto(6, 5, Kind(Grass));
        map.PaintTileAuto(6, 5, Kind(Bush));

        // the grass and the bush of (5,5) both have a neighbour on the right now
        Assert.Equal(AutotileShapes.Floor(false, false, false, true, false, false, false, false), TileIds.Shape(map.GetTile(5, 5, 0)));
        Assert.Equal(AutotileShapes.Floor(false, false, false, true, false, false, false, false), TileIds.Shape(map.GetTile(5, 5, 1)));
        Assert.Equal(AutotileShapes.Floor(false, false, true, false, false, false, false, false), TileIds.Shape(map.GetTile(6, 5, 1)));
    }

    // ---- VX Ace ----

    [Fact]
    public void VxGroundIsOnLayer0AnOverlayOnLayer1AndBToEOnLayer2()
    {
        var map = Vx();
        map.PaintTileAuto(0, 0, Kind(Grass));
        map.PaintTileAuto(0, 0, Kind(Bush));
        map.PaintTileAuto(0, 0, 88);
        map.PaintTileAuto(1, 0, Kind(Bush));

        Assert.Equal([K(Grass), K(Bush), 88], Layers(map, 0));
        Assert.Equal([0, K(Bush), 0], Layers(map, 1));
    }

    [Fact]
    public void VxATileOfBToEReplacesTheOneThatWasThereAndGroundRemovesTheOverlay()
    {
        var map = Vx();
        map.PaintTileAuto(0, 0, 88);
        map.PaintTileAuto(0, 0, 89);
        map.PaintTileAuto(1, 0, Kind(Grass));
        map.PaintTileAuto(1, 0, Kind(Bush));
        map.PaintTileAuto(1, 0, Kind(17));

        Assert.Equal([0, 0, 89], Layers(map, 0));
        Assert.Equal([K(17), 0, 0], Layers(map, 1));
    }

    // ---- what is refused ----

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1024)]
    [InlineData(1535)]
    [InlineData(TileIds.Max)]
    public void ANumberThatIsNotATileIsRefused(int tileId)
    {
        var map = Mv();

        Assert.Throws<ArgumentOutOfRangeException>(() => map.PaintTileAuto(0, 0, tileId));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ASquareOutsideOfTheMapIsRefused(Version v)
    {
        var map = v.New<IMap>();

        Assert.Throws<ArgumentOutOfRangeException>(() => map.PaintTileAuto(map.Width, 0, 88));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.PaintTileAuto(0, -1, 88));
    }

    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });
}
