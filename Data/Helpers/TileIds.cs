namespace RPGMDataKit.Data.Helpers;

/// <summary>
/// The sheet of a tileset a tile comes from. The number is the place of the sheet in <c>ITileSet.TilesetNames</c>.
/// </summary>
public enum TileSheet
{
    A1 = 0,
    A2 = 1,
    A3 = 2,
    A4 = 3,
    A5 = 4,
    B = 5,
    C = 6,
    D = 7,
    E = 8,
}

/// <summary>
/// What a tile id means. The same in VX Ace and MV (<c>Tilemap.TILE_ID_*</c> and the functions of <c>Tilemap</c> in MV); see <c>.claude-docs/maps-and-tiles.md</c>.
/// <list type="bullet">
/// <item>0 is nothing; B, C, D, E are 256 normal tiles each (0-1023), A5 is 1536-1663 (the range goes to 2047), the ids 1024-1535 are not used.</item>
/// <item>An autotile is an id of 2048 and above: <c>2048 + kind * 48 + shape</c>. The kinds are numbered from A1: A1 0-15, A2 16-47, A3 48-79, A4 80-127.</item>
/// </list>
/// </summary>
public static class TileIds
{
    public const int B = 0;
    public const int C = 256;
    public const int D = 512;
    public const int E = 768;
    public const int A5 = 1536;
    public const int A1 = 2048;
    public const int A2 = 2816;
    public const int A3 = 4352;
    public const int A4 = 5888;

    /// <summary>One past the last tile id: a tileset has this many flags.</summary>
    public const int Max = 8192;

    /// <summary>The number of shapes of an autotile kind.</summary>
    public const int ShapesPerKind = 48;

    /// <summary>The number of autotile kinds (A1 16, A2 32, A3 32, A4 48).</summary>
    public const int AutotileKinds = (Max - A1) / ShapesPerKind;

    /// <summary>The tile is drawn: it is not 0 and not past the last id.</summary>
    public static bool IsVisible(int tileId) => tileId > 0 && tileId < Max;

    /// <summary>An autotile: an id of A1 and above (as the engine tells it).</summary>
    public static bool IsAutotile(int tileId) => tileId >= A1;

    /// <summary>The kind of an autotile (0-127), counted from A1. For an id that is not an autotile the result is negative.</summary>
    public static int Kind(int tileId) => FloorDiv(tileId - A1, ShapesPerKind);

    /// <summary>The shape of an autotile (0-47): which neighbours are of the same kind. For an id that is not an autotile the result is meaningless.</summary>
    public static int Shape(int tileId) => Mod(tileId - A1, ShapesPerKind);

    /// <summary>The id of the autotile of the kind (0-127) in the shape (0-47).</summary>
    public static int MakeAutotile(int kind, int shape)
    {
        if ((uint)kind >= AutotileKinds)
            throw new ArgumentOutOfRangeException(nameof(kind), kind, $"An autotile kind is 0-{AutotileKinds - 1}");
        if ((uint)shape >= ShapesPerKind)
            throw new ArgumentOutOfRangeException(nameof(shape), shape, $"An autotile shape is 0-{ShapesPerKind - 1}");

        return A1 + kind * ShapesPerKind + shape;
    }

    public static bool IsA1(int tileId) => tileId >= A1 && tileId < A2;

    public static bool IsA2(int tileId) => tileId >= A2 && tileId < A3;

    public static bool IsA3(int tileId) => tileId >= A3 && tileId < A4;

    public static bool IsA4(int tileId) => tileId >= A4 && tileId < Max;

    /// <summary>A5, 1536-2047 (the sheet has 128 tiles, 1536-1663).</summary>
    public static bool IsA5(int tileId) => tileId >= A5 && tileId < A1;

    /// <summary>
    /// The sheet of the tile, or null for 0, an id past the last one and the unused ids (1024-1535). The tiles of B-E and A5 are normal tiles,
    /// the ones of A1-A4 autotiles.
    /// </summary>
    public static TileSheet? SheetOf(int tileId)
    {
        if (tileId <= 0 || tileId >= Max)
            return null;
        if (tileId < E + 256)
            return (TileSheet)(5 + tileId / 256);
        if (IsA5(tileId))
            return TileSheet.A5;
        if (IsA1(tileId))
            return TileSheet.A1;
        if (IsA2(tileId))
            return TileSheet.A2;
        if (IsA3(tileId))
            return TileSheet.A3;
        if (IsA4(tileId))
            return TileSheet.A4;

        return null;
    }

    /// <summary>
    /// The place of a normal tile (B-E, A5) on its sheet, in tiles: B-E have 16 x 16, A5 has 8 x 16 (the ids 0-127 of a sheet are its left half,
    /// 128-255 the right one). For an autotile, which is cut from the sheet by its shape, the result is meaningless.
    /// </summary>
    public static (int Column, int Row) NormalTileCell(int tileId) =>
        (tileId / 128 % 2 * 8 + tileId % 8, tileId % 256 / 8 % 16);

    /// <summary>Water: A1 except its kinds 2 and 3 (the waterfalls count).</summary>
    public static bool IsWater(int tileId) => IsA1(tileId) && !(tileId >= A1 + 96 && tileId < A1 + 192);

    /// <summary>A waterfall: the odd kinds of A1 from the kind 4.</summary>
    public static bool IsWaterfall(int tileId) => tileId >= A1 + 192 && tileId < A2 && Kind(tileId) % 2 == 1;

    /// <summary>Ground: A1, A2 and A5.</summary>
    public static bool IsGround(int tileId) => IsA1(tileId) || IsA2(tileId) || IsA5(tileId);

    /// <summary>A3 and A4: the shadow of a tile on the layer below is not drawn over them.</summary>
    public static bool IsShadowing(int tileId) => IsA3(tileId) || IsA4(tileId);

    /// <summary>A roof: the kinds 0-7 of each 16 of A3.</summary>
    public static bool IsRoof(int tileId) => IsA3(tileId) && Kind(tileId) % 16 < 8;

    /// <summary>The top of a wall: the kinds 0-7 of each 16 of A4.</summary>
    public static bool IsWallTop(int tileId) => IsA4(tileId) && Kind(tileId) % 16 < 8;

    /// <summary>The side of a wall: the kinds 8-15 of each 16 of A3 and A4.</summary>
    public static bool IsWallSide(int tileId) => (IsA3(tileId) || IsA4(tileId)) && Kind(tileId) % 16 >= 8;

    public static bool IsWall(int tileId) => IsWallTop(tileId) || IsWallSide(tileId);

    /// <summary>An autotile of 48 shapes: water, ground, the top of a wall.</summary>
    public static bool IsFloorTypeAutotile(int tileId) => (IsA1(tileId) && !IsWaterfall(tileId)) || IsA2(tileId) || IsWallTop(tileId);

    /// <summary>An autotile of 16 shapes: a roof or the side of a wall.</summary>
    public static bool IsWallTypeAutotile(int tileId) => IsRoof(tileId) || IsWallSide(tileId);

    /// <summary>An autotile of 4 shapes: a waterfall.</summary>
    public static bool IsWaterfallTypeAutotile(int tileId) => IsWaterfall(tileId);

    private static int FloorDiv(int value, int divisor) => (int)Math.Floor((double)value / divisor);

    private static int Mod(int value, int divisor) => (value % divisor + divisor) % divisor;
}
