using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// The shadow beside walls (<see cref="MapShadowExtensions"/>). The picture of walls and the picture of shadows are those of a map that was painted in the editor of MV (walls of A3 and A4, then looked at square by square).
/// </summary>
public sealed class MapShadowTests
{
    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private const int WallSide = 88;   // A4
    private const int Roof = 48;       // A3
    private const int Grass = 16;      // A2

    private static int Kind(int kind) => TileIds.MakeAutotile(kind, 0);

    // the walls of the map that was painted in the editor (W) and the shadows it had (5 = the left half)
    private static readonly string[] Walls =
    [
        ".................",
        ".W.W....W..W.....",
        ".W.W.............",
        "......W..........",
        ".W.W..W.WWW......",
        ".W.W....WWW......",
        ".W.W....WWW......",
        "........WWW......",
        "....W...WWW......",
        "....WW........W..",
        "....WW........W..",
        ".................",
        ".................",
    ];

    private static readonly string[] Shadows =
    [
        ".................",
        ".................",
        "..5.5............",
        ".................",
        ".......5.........",
        "..5.5......5.....",
        "..5.5......5.....",
        "...........5.....",
        "...........5.....",
        ".................",
        "......5........5.",
        ".................",
        ".................",
    ];

    private static IMap Painted(Version v, int kind = WallSide)
    {
        var map = v.New<IMap>();
        for (var y = 0; y < Walls.Length; y++)
        {
            for (var x = 0; x < Walls[y].Length; x++)
            {
                if (Walls[y][x] == 'W')
                    map.PaintTileAuto(x, y, Kind(kind));
            }
        }

        return map;
    }

    private static string[] ShadowPicture(IMap map) =>
        Enumerable.Range(0, map.Height).Select(y => new string(Enumerable.Range(0, map.Width).Select(x => map.GetShadow(x, y) is var s && s != 0 ? (char)('0' + s) : '.').ToArray())).ToArray();

    [Theory]
    [MemberData(nameof(Versions))]
    public void PaintingWallsGivesTheShadowsOfTheEditor(Version v)
    {
        Assert.Equal(Shadows, ShadowPicture(Painted(v)));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ARoofCastsTheSameShadowAsAWall(Version v)
    {
        Assert.Equal(Shadows, ShadowPicture(Painted(v, Roof)));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void UpdatingAllTheShadowsGivesTheSameAsPainting(Version v)
    {
        var map = v.New<IMap>();
        for (var y = 0; y < Walls.Length; y++)
        {
            for (var x = 0; x < Walls[y].Length; x++)
            {
                if (Walls[y][x] == 'W')
                    map.Data[x, y, 0] = Kind(WallSide);    // written to the table itself: no shadows are made
            }
        }

        Assert.All(ShadowPicture(map), row => Assert.Equal(new string('.', 17), row));
        Assert.Equal(13, map.UpdateWallShadows());
        Assert.Equal(Shadows, ShadowPicture(map));
        Assert.Equal(0, map.UpdateWallShadows());
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheShadowIsTheLeftHalfOfTheSquare(Version v)
    {
        var map = Painted(v);

        Assert.Equal(ShadowFlag.TopLeft | ShadowFlag.BottomLeft, map.GetShadowFlags(2, 2));
        Assert.Equal(5, map.GetShadow(2, 2));
        Assert.Equal(MapShadowExtensions.WallShadow, map.WallShadowAt(2, 2));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ATileOfAWallOnAnyLayerCountsAndOutsideOfTheMapDoesNot(Version v)
    {
        var map = v.New<IMap>();
        map.SetTile(3, 3, map.TileLayerCount() - 1, Kind(WallSide));
        map.SetTile(3, 4, 1, Kind(Roof));
        map.SetTile(4, 3, 0, Kind(Grass));

        Assert.True(map.HasWall(3, 3));
        Assert.True(map.HasWall(3, 4));
        Assert.False(map.HasWall(4, 3));
        Assert.False(map.HasWall(-1, 0));
        Assert.False(map.HasWall(0, map.Height));
        Assert.Equal(ShadowFlag.None, map.WallShadowAt(0, 0));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void RemovingAWallOrPaintingOverItTakesTheShadowAway(Version v)
    {
        var map = v.New<IMap>();
        map.PaintTileAuto(2, 2, Kind(WallSide));
        map.PaintTileAuto(2, 3, Kind(WallSide));
        Assert.Equal(5, map.GetShadow(3, 3));

        map.PaintTileAuto(2, 3, Kind(Grass));      // the wall is gone: the shadow beside it goes
        Assert.Equal(0, map.GetShadow(3, 3));

        map.PaintTileAuto(2, 3, Kind(WallSide));
        Assert.Equal(5, map.GetShadow(3, 3));
        map.PaintTileAuto(3, 3, Kind(WallSide));   // a wall on the square itself: no shadow there
        Assert.Equal(0, map.GetShadow(3, 3));
        map.PaintTileAuto(3, 3, Kind(Grass));      // the ground is back: so is the shadow
        Assert.Equal(5, map.GetShadow(3, 3));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void PaintingATileOnALayerOfYourOwnUpdatesTheShadowsToo(Version v)
    {
        var map = v.New<IMap>();
        map.PaintTile(2, 2, 0, Kind(Roof));
        map.PaintTile(2, 3, 0, Kind(Roof));

        Assert.Equal(5, map.GetShadow(3, 3));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void SetTileKeepsTheShadowsBesideWallsToo(Version v)
    {
        var map = v.New<IMap>();
        for (var y = 0; y < Walls.Length; y++)
        {
            for (var x = 0; x < Walls[y].Length; x++)
            {
                if (Walls[y][x] == 'W')
                    map.SetTile(x, y, 0, Kind(WallSide));
            }
        }

        Assert.Equal(Shadows, ShadowPicture(map));

        map.SetTile(2, 3, 0, 0);          // a wall is taken away (not a tile of A): the shadow of the square to its right goes all the same
        Assert.Equal(0, map.GetShadow(3, 3));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ATileOfAClearsTheShadowsOfSixSquaresAndWorksThemOutAgainFromTheWalls(Version v)
    {
        // what the editor of MV does when a tile of A is put on a square: the square, above, below, to the right, upper right, lower right
        var map = v.New<IMap>();
        var around = new[] { (5, 5), (5, 4), (5, 6), (6, 5), (6, 4), (6, 6) };
        var outside = new[] { (4, 5), (4, 4), (4, 6), (7, 5), (5, 3), (5, 7), (6, 7), (6, 3) };
        foreach (var (x, y) in around.Concat(outside))
            map.SetShadow(x, y, ShadowFlag.TopRight);     // shadows drawn by hand

        map.SetTile(5, 5, 0, Kind(Grass));

        Assert.All(around, p => Assert.Equal(0, map.GetShadow(p.Item1, p.Item2)));
        Assert.All(outside, p => Assert.Equal((int)ShadowFlag.TopRight, map.GetShadow(p.Item1, p.Item2)));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ATileOfAAlsoMakesTheWallShadowOfTheSixSquares(Version v)
    {
        var map = v.New<IMap>();
        map.Data[4, 6, 0] = Kind(WallSide);     // the walls are in the table, no shadows yet
        map.Data[4, 5, 0] = Kind(WallSide);

        map.SetTile(8, 8, 0, Kind(Grass));      // far away: nothing happens beside the walls
        Assert.Equal(0, map.GetShadow(5, 6));

        map.SetTile(5, 5, 0, Kind(Grass));      // the squares (5,5) and (5,6) are among the six: (5,6) has walls at its left and up-left
        Assert.Equal(0, map.GetShadow(5, 5));   // up-left of (5,5) is not a wall
        Assert.Equal(5, map.GetShadow(5, 6));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ATileThatIsNotOfAKeepsTheShadowsUnlessItTookAWallAway(Version v)
    {
        var map = v.New<IMap>();
        map.SetShadow(5, 5, ShadowFlag.TopRight);

        map.SetTile(5, 5, 2, 88);                  // a tile of B on the square: shadows are not worked out again
        map.SetTile(4, 5, 2, 88);
        Assert.Equal((int)ShadowFlag.TopRight, map.GetShadow(5, 5));

        map.SetTile(4, 5, 0, Kind(WallSide));      // a wall at its left, nothing up-left: the shadow was cleared and the rule gives none
        Assert.Equal(0, map.GetShadow(5, 5));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void TheExplicitUpdateLeavesAShadowOfAnotherShapeAlone(Version v)
    {
        var map = v.New<IMap>();
        map.Data[2, 2, 0] = Kind(WallSide);
        map.Data[2, 3, 0] = Kind(WallSide);
        map.Data[2, 4, 0] = Kind(WallSide);
        map.SetShadow(3, 3, ShadowFlag.TopRight);       // by hand, where a wall would put its shadow
        map.SetShadow(3, 4, ShadowFlag.All);
        map.SetShadow(9, 9, ShadowFlag.BottomLeft);     // far from any wall

        Assert.Equal(0, map.UpdateWallShadows());

        Assert.Equal((int)ShadowFlag.TopRight, map.GetShadow(3, 3));
        Assert.Equal((int)ShadowFlag.All, map.GetShadow(3, 4));
        Assert.Equal((int)ShadowFlag.BottomLeft, map.GetShadow(9, 9));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void ABlockOfGroundOrObjectsBesideAWallChangesNothing(Version v)
    {
        var map = v.New<IMap>();
        map.PaintTileAuto(2, 2, Kind(WallSide));
        map.PaintTileAuto(2, 3, Kind(WallSide));
        map.SetShadow(3, 3, ShadowFlag.TopRight);   // drawn by hand

        map.PaintTileAuto(3, 3, 88);                // an object on the square is not a wall: the shadow is left as it is
        Assert.Equal((int)ShadowFlag.TopRight, map.GetShadow(3, 3));
    }

    [Fact]
    public void TheRegionOfVxAceSharesTheCellWithTheShadowAndStays()
    {
        var map = Version.VXAce.New<IMap>();
        map.SetRegion(3, 3, 9);

        map.PaintTileAuto(2, 2, Kind(WallSide));
        map.PaintTileAuto(2, 3, Kind(WallSide));

        Assert.Equal(5, map.GetShadow(3, 3));
        Assert.Equal(9, map.GetRegion(3, 3));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void UpdatingAPartMustBeInsideOfTheMap(Version v)
    {
        var map = v.New<IMap>();

        Assert.Throws<ArgumentOutOfRangeException>(() => map.UpdateWallShadows(0, 0, map.Width + 1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => map.UpdateWallShadow(map.Width, 0));
    }
}
