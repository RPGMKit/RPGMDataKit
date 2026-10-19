using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// The shape of an autotile from its neighbours (<see cref="AutotileShapes"/>, <see cref="MapAutotileExtensions"/>). The rule was compared with the maps of real projects of both editors
/// (see <c>.claude-docs/maps-and-tiles.md</c>); these tests keep its parts.
/// </summary>
public sealed class AutotileTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    // an autotile of the kind (A2 ground 16-47; A1 water 0-1, 4...; A3 roofs 48-55, walls 56-63; A4 tops 80-87, sides 88-95) with whatever shape
    private static int A2(int kind = 16) => TileIds.MakeAutotile(kind, 0);

    private const int Water = 0;
    private const int Waterfall = 5;
    private const int Roof = 48;
    private const int WallSide = 56;
    private const int WallTop = 80;
    private const int WallSideA4 = 88;

    // a block of the tile at x 1..3, y 1..3 on the layer 0
    private static IMap Block(Version v, int tile, int x = 1, int y = 1, int width = 3, int height = 3)
    {
        var map = v.New<IMap>();
        for (var j = y; j < y + height; j++)
        {
            for (var i = x; i < x + width; i++)
                map.SetTile(i, j, 0, tile);
        }

        return map;
    }

    // ---- the shape from the 8 neighbours ----

    [Fact]
    public void AllNeighboursConnectedIsShape0AndNoneIs46()
    {
        Assert.Equal(0, AutotileShapes.Floor(true, true, true, true, true, true, true, true));
        Assert.Equal(46, AutotileShapes.Floor(false, false, false, false, false, false, false, false));
        Assert.Equal(46, AutotileShapes.Floor(false, false, false, false, true, true, true, true));
    }

    [Theory]
    [InlineData(false, true, true, true, 1)]   // a notch at the top left
    [InlineData(true, false, true, true, 2)]   // top right
    [InlineData(true, true, false, true, 4)]   // bottom right
    [InlineData(true, true, true, false, 8)]   // bottom left
    public void ACornerThatIsNotConnectedMakesANotchWhenBothSidesOfItAre(bool upLeft, bool upRight, bool downRight, bool downLeft, int shape)
    {
        Assert.Equal(shape, AutotileShapes.Floor(true, true, true, true, upLeft, upRight, downLeft, downRight));
    }

    [Fact]
    public void ACornerNextToASideThatIsNotConnectedIsNotLookedAt()
    {
        // the top is not connected: the two corners above it are hidden, whatever they are
        var shapes = new HashSet<int>();
        foreach (var upLeft in new[] { false, true })
        {
            foreach (var upRight in new[] { false, true })
                shapes.Add(AutotileShapes.Floor(false, true, true, true, upLeft, upRight, true, true));
        }

        Assert.Equal(new[] { 20 }, shapes.ToArray());
    }

    [Fact]
    public void The256CombinationsOfNeighboursGive47ShapesFrom0To46()
    {
        var shapes = new HashSet<int>();
        for (var mask = 0; mask < 256; mask++)
        {
            bool Bit(int i) => (mask >> i & 1) != 0;
            shapes.Add(AutotileShapes.Floor(Bit(0), Bit(1), Bit(2), Bit(3), Bit(4), Bit(5), Bit(6), Bit(7)));
        }

        Assert.Equal(Enumerable.Range(0, 47), shapes.Order());
    }

    [Theory]
    [InlineData(false, true, true, true, 16)]    // only the left is not connected
    [InlineData(true, false, true, true, 20)]    // up
    [InlineData(true, true, false, true, 24)]    // right: the bits are (left, up, right, down)
    [InlineData(true, true, true, false, 28)]    // down
    public void OneSideThatIsNotConnectedStartsItsOwnFamily(bool left, bool up, bool right, bool down, int shape)
    {
        Assert.Equal(shape, AutotileShapes.Floor(up, down, left, right, true, true, true, true));
    }

    [Fact]
    public void TwoOppositeSidesAndThreeSidesThatAreNotConnected()
    {
        Assert.Equal(32, AutotileShapes.Floor(true, true, false, false, true, true, true, true));
        Assert.Equal(33, AutotileShapes.Floor(false, false, true, true, true, true, true, true));
        Assert.Equal(42, AutotileShapes.Floor(false, true, false, false, true, true, true, true));    // only the bottom is connected
        Assert.Equal(43, AutotileShapes.Floor(false, false, false, true, true, true, true, true));    // only the right
        Assert.Equal(44, AutotileShapes.Floor(true, false, false, false, true, true, true, true));    // only the top
        Assert.Equal(45, AutotileShapes.Floor(false, false, true, false, true, true, true, true));    // only the left
    }

    [Fact]
    public void AWallHasABitForEachSideThatIsNotConnected()
    {
        Assert.Equal(0, AutotileShapes.Wall(true, true, true, true));
        Assert.Equal(1, AutotileShapes.Wall(true, true, false, true));   // left
        Assert.Equal(2, AutotileShapes.Wall(false, true, true, true));   // up
        Assert.Equal(4, AutotileShapes.Wall(true, true, true, false));   // right
        Assert.Equal(8, AutotileShapes.Wall(true, false, true, true));   // down
        Assert.Equal(15, AutotileShapes.Wall(false, false, false, false));
    }

    [Fact]
    public void AWaterfallHasABitForEachSideThatIsNotConnected()
    {
        Assert.Equal(0, AutotileShapes.Waterfall(true, true));
        Assert.Equal(1, AutotileShapes.Waterfall(false, true));
        Assert.Equal(2, AutotileShapes.Waterfall(true, false));
        Assert.Equal(3, AutotileShapes.Waterfall(false, false));
    }

    // ---- on a map ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void ABlockOfGroundGetsTheShapesOfItsEdgesAndCorners(Version v)
    {
        var map = Block(v, A2());
        map.UpdateAutotiles();

        int Shape(int x, int y) => TileIds.Shape(map.GetTile(x, y, 0));
        Assert.Equal(34, Shape(1, 1));   // top left
        Assert.Equal(20, Shape(2, 1));   // top
        Assert.Equal(36, Shape(3, 1));   // top right
        Assert.Equal(16, Shape(1, 2));   // left
        Assert.Equal(0, Shape(2, 2));    // inside
        Assert.Equal(24, Shape(3, 2));   // right
        Assert.Equal(40, Shape(1, 3));   // bottom left
        Assert.Equal(28, Shape(2, 3));   // bottom
        Assert.Equal(38, Shape(3, 3));   // bottom right
        Assert.Equal(TileIds.Kind(A2()), TileIds.Kind(map.GetTile(2, 2, 0)));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ATileWithNothingRoundItIsShape46(Version v)
    {
        var map = Block(v, A2(), 5, 5, 1, 1);

        Assert.Equal(46, map.AutotileShapeAt(5, 5, 0));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void AnotherKindOfGroundIsNotConnectedAndTheSameKindIs(Version v)
    {
        var map = Block(v, A2(16), 1, 1, 2, 1);
        map.SetTile(3, 1, 0, A2(17));
        map.SetTile(4, 1, 0, A2(16));

        Assert.Equal(43, map.AutotileShapeAt(1, 1, 0));   // only its right is connected
        Assert.Equal(45, map.AutotileShapeAt(2, 1, 0));   // only its left: the other kind at its right is not
        Assert.Equal(46, map.AutotileShapeAt(4, 1, 0));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void OnlyTheSameLayerIsLookedAt(Version v)
    {
        var map = v.New<IMap>();
        map.SetTile(5, 5, 0, A2());
        map.SetTile(6, 5, 1, A2());

        Assert.Equal(46, map.AutotileShapeAt(5, 5, 0));
        Assert.Equal(46, map.AutotileShapeAt(6, 5, 1));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheEdgeOfTheMapIsConnectedForGroundWaterAndTheTopOfAWall(Version v)
    {
        foreach (var kind in new[] { A2(), TileIds.MakeAutotile(Water, 0), TileIds.MakeAutotile(WallTop, 0) })
        {
            var map = v.New<IMap>();
            for (var y = 0; y < map.Height; y++)
            {
                for (var x = 0; x < map.Width; x++)
                    map.SetTile(x, y, 0, kind);
            }

            Assert.Equal(0, map.AutotileShapeAt(0, 0, 0));
            Assert.Equal(0, map.AutotileShapeAt(map.Width - 1, map.Height - 1, 0));
        }
    }

    [Fact]
    public void TheEdgeOfTheMapIsConnectedToTheSidesForAWallInVxAceAndNotInMv()
    {
        foreach (var kind in new[] { Roof, WallSide, WallSideA4 })
        {
            var tile = TileIds.MakeAutotile(kind, 0);
            var vx = Version.VXAce.New<IMap>();
            var mv = Version.Mv.New<IMap>();
            foreach (var map in new[] { vx, mv })
            {
                for (var x = 0; x < map.Width; x++)
                {
                    for (var y = 0; y < map.Height; y++)
                        map.SetTile(x, y, 0, tile);
                }
            }

            // a corner has its left and top at the edge: VX Ace joins the left, never the top
            Assert.Equal(2, vx.AutotileShapeAt(0, 0, 0));
            Assert.Equal(3, mv.AutotileShapeAt(0, 0, 0));
            Assert.Equal(8, vx.AutotileShapeAt(vx.Width - 1, vx.Height - 1, 0));
            Assert.Equal(12, mv.AutotileShapeAt(mv.Width - 1, mv.Height - 1, 0));
        }
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void WaterIsConnectedToAWaterfallAboveAndBelowIt(Version v)
    {
        var map = v.New<IMap>();
        map.SetTile(5, 4, 0, TileIds.MakeAutotile(Water, 0));
        map.SetTile(5, 5, 0, TileIds.MakeAutotile(Waterfall, 0));
        map.SetTile(5, 6, 0, TileIds.MakeAutotile(Water, 0));

        Assert.Equal(AutotileShapes.Floor(false, true, false, false, false, false, false, false), map.AutotileShapeAt(5, 4, 0));
        Assert.Equal(AutotileShapes.Floor(true, false, false, false, false, false, false, false), map.AutotileShapeAt(5, 6, 0));
        Assert.Equal(3, map.AutotileShapeAt(5, 5, 0));   // a waterfall looks at its sides only: nothing is there
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheSideOfAWallIsConnectedToTheTopOfAnyWallAtItsSidesOnly(Version v)
    {
        var map = v.New<IMap>();
        map.SetTile(4, 5, 0, TileIds.MakeAutotile(WallTop, 0));
        map.SetTile(5, 5, 0, TileIds.MakeAutotile(WallSideA4, 0));
        map.SetTile(6, 5, 0, TileIds.MakeAutotile(WallTop + 3, 0));
        map.SetTile(5, 4, 0, TileIds.MakeAutotile(WallTop, 0));
        map.SetTile(5, 6, 0, TileIds.MakeAutotile(WallTop, 0));

        Assert.Equal(2 + 8, map.AutotileShapeAt(5, 5, 0));   // left and right are connected, up and down (a top) are not
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ARoofIsConnectedToItsOwnKindOnly(Version v)
    {
        var map = v.New<IMap>();
        map.SetTile(5, 5, 0, TileIds.MakeAutotile(Roof, 0));
        map.SetTile(6, 5, 0, TileIds.MakeAutotile(Roof + 1, 0));
        map.SetTile(4, 5, 0, TileIds.MakeAutotile(WallTop, 0));

        Assert.Equal(15, map.AutotileShapeAt(5, 5, 0));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ATileThatIsNotAnAutotileHasNoShape(Version v)
    {
        var map = v.New<IMap>();
        map.SetTile(2, 2, 0, 3);

        Assert.Throws<ArgumentException>(() => map.AutotileShapeAt(2, 2, 0));
        Assert.Throws<ArgumentException>(() => map.AutotileShapeAt(3, 2, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.AutotileShapeAt(map.Width, 0, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.AutotileShapeAt(0, 0, map.TileLayerCount()));
        Assert.False(map.UpdateAutotile(2, 2, 0));
        Assert.Equal(3, map.GetTile(2, 2, 0));
    }

    // ---- painting ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void PaintingATileShapesItAndTheTilesAroundIt(Version v)
    {
        var map = v.New<IMap>();
        var ground = A2();
        map.PaintTile(5, 5, 0, ground);
        Assert.Equal(46, TileIds.Shape(map.GetTile(5, 5, 0)));

        map.PaintTile(6, 5, 0, ground);
        Assert.Equal(AutotileShapes.Floor(false, false, false, true, false, false, false, false), TileIds.Shape(map.GetTile(5, 5, 0)));
        Assert.Equal(AutotileShapes.Floor(false, false, true, false, false, false, false, false), TileIds.Shape(map.GetTile(6, 5, 0)));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ThePaintedShapeIsNotTheOneInTheId(Version v)
    {
        var map = v.New<IMap>();

        map.PaintTile(5, 5, 0, TileIds.MakeAutotile(16, 33));

        Assert.Equal(TileIds.MakeAutotile(16, 46), map.GetTile(5, 5, 0));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ErasingATileOrPaintingAnotherOverItReshapesTheTilesAroundIt(Version v)
    {
        var map = Block(v, A2());
        map.UpdateAutotiles();
        Assert.Equal(0, TileIds.Shape(map.GetTile(2, 2, 0)));

        map.PaintTile(3, 2, 0, 0);
        Assert.Equal(24, TileIds.Shape(map.GetTile(2, 2, 0)));   // its right is not connected now
        Assert.Equal(0, map.GetTile(3, 2, 0));

        map.PaintTile(3, 2, 0, 7);
        Assert.Equal(7, map.GetTile(3, 2, 0));
        Assert.Equal(24, TileIds.Shape(map.GetTile(2, 2, 0)));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void PaintingOnOneLayerLeavesTheOthersAlone(Version v)
    {
        var map = v.New<IMap>();
        map.PaintTile(5, 5, 1, A2());
        map.PaintTile(5, 5, 0, A2());
        map.PaintTile(6, 5, 0, A2());

        Assert.Equal(46, TileIds.Shape(map.GetTile(5, 5, 1)));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void PaintingAtTheEdgeOfTheMapIsFine(Version v)
    {
        var map = v.New<IMap>();

        map.PaintTile(0, 0, 0, A2());
        map.PaintTile(map.Width - 1, map.Height - 1, 0, A2());

        Assert.Equal(TileIds.Kind(A2()), TileIds.Kind(map.GetTile(0, 0, 0)));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.PaintTile(map.Width, 0, 0, A2()));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.PaintTile(0, 0, map.TileLayerCount(), A2()));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.PaintTile(0, 0, 0, TileIds.Max));
    }

    // ---- updating ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void UpdatingGivesTheCountOfTilesThatChangedAndASecondUpdateChangesNothing(Version v)
    {
        var map = Block(v, A2());

        Assert.Equal(8, map.UpdateAutotiles());   // the 8 tiles round the middle one: it already is shape 0
        Assert.Equal(0, map.UpdateAutotiles());
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void APartOfTheMapCanBeUpdatedAndMustBeInsideOfIt(Version v)
    {
        var map = Block(v, A2());

        Assert.Equal(1, map.UpdateAutotiles(1, 1, 1, 1));
        Assert.Equal(34, TileIds.Shape(map.GetTile(1, 1, 0)));
        Assert.Equal(0, TileIds.Shape(map.GetTile(2, 1, 0)));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.UpdateAutotiles(0, 0, map.Width + 1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.UpdateAutotiles(-1, 0, 1, 1));
    }
}
