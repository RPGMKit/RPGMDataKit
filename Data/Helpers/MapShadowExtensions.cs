using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;

namespace RPGMDataKit.Data.Helpers;

/// <summary>
/// The shadow that the editors put beside walls by themselves. A tile of A3 or A4 (a roof, a wall of a house, the top or the side of a wall) casts a shadow on the square to its right: the left half of that square is dark
/// (<see cref="WallShadow"/>, the value 5). The rule, from a map painted in the editor of MV (the maintainer says that the editor of VX Ace has the same rules, and the maps it saved fit them, see <c>.claude-docs/maps-and-tiles.md</c>):
/// <list type="bullet">
/// <item>a square with a tile of A3 or A4 on any layer has no such shadow;</item>
/// <item>any other square has it when the square to its left <b>and</b> the square above that one (the diagonal, up-left) both have a tile of A3 or A4 on some layer.</item>
/// </list>
/// So the top row of a wall casts nothing beside it: a shadow starts one row lower.
/// <para>
/// <see cref="MapTileExtensions.SetTile"/> (and so every painting method) keeps these shadows as the editors do (found by the maintainer in the editor of MV): when a tile of A (A1-A5) is put on a square,
/// the shadows of the square, of the squares above and below it, to its right, and to its upper right and lower right are cleared and worked out again from the walls alone: <b>a shadow drawn by hand
/// on one of these squares is lost</b>. A tile that is not of A changes the shadows only if it took a wall away.
/// The explicit <see cref="UpdateWallShadow"/> / <see cref="UpdateWallShadows()"/> are gentler: they change only an empty shadow and the wall shadow itself, and leave a shadow of another shape alone.
/// </para>
/// </summary>
public static class MapShadowExtensions
{
    /// <summary>The shadow of a wall: the left half of the square (the top left and the bottom left quarters).</summary>
    public const ShadowFlag WallShadow = ShadowFlag.TopLeft | ShadowFlag.BottomLeft;

    /// <summary>The square has a tile of A3 or A4 on some layer of tiles; false outside of the map.</summary>
    public static bool HasWall(this IMap map, int x, int y)
    {
        if (x < 0 || y < 0 || x >= map.Width || y >= map.Height)
            return false;

        for (var layer = 0; layer < map.TileLayerCount(); layer++)
        {
            var tile = map.Data[x, y, layer];
            if (TileIds.IsShadowing(tile) && tile < TileIds.Max)
                return true;
        }

        return false;
    }

    /// <summary>The shadow that the walls round the square give it: <see cref="WallShadow"/> or none.</summary>
    public static ShadowFlag WallShadowAt(this IMap map, int x, int y) =>
        !map.HasWall(x, y) && map.HasWall(x - 1, y) && map.HasWall(x - 1, y - 1) ? WallShadow : ShadowFlag.None;

    /// <summary>
    /// Brings the shadow of the square in line with its walls (<see cref="WallShadowAt"/>) <b>without touching a shadow that is of another shape</b>: an empty square that should have the wall shadow gets it,
    /// and a square that has the wall shadow and should not loses it; a square with any other shadow (drawn by hand: a quarter, the right half, ...) is left as it is. A shadow that is drawn by hand as the left half
    /// cannot be told from the one of a wall, and goes when its square no longer has a wall to the left. Returns whether the shadow changed.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The square is outside of the map.</exception>
    public static bool UpdateWallShadow(this IMap map, int x, int y)
    {
        var wanted = map.WallShadowAt(x, y);
        var current = map.GetShadowFlags(x, y);
        if (current == wanted || (current != ShadowFlag.None && current != WallShadow))
            return false;

        map.SetShadow(x, y, wanted);

        return true;
    }

    /// <summary>
    /// Brings the shadows of every square of the map in line with the walls (see <see cref="UpdateWallShadow"/>: the shadows of other shapes are not touched). Returns how many squares changed.
    /// </summary>
    public static int UpdateWallShadows(this IMap map) => map.UpdateWallShadows(0, 0, map.Width, map.Height);

    /// <summary>Brings the shadows of a part of the map (x, y and the size in squares) in line with the walls (see <see cref="UpdateWallShadow"/>). Returns how many squares changed.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The part is not inside of the map.</exception>
    public static int UpdateWallShadows(this IMap map, int x, int y, int width, int height)
    {
        if (width < 0 || height < 0 || x < 0 || y < 0 || x + width > map.Width || y + height > map.Height)
            throw new ArgumentOutOfRangeException(nameof(width), $"The part {x},{y} {width}x{height} is not inside of the map ({map.Width}x{map.Height})");

        var changed = 0;
        for (var row = y; row < y + height; row++)
        {
            for (var column = x; column < x + width; column++)
                changed += map.UpdateWallShadow(column, row) ? 1 : 0;
        }

        return changed;
    }

    // what the editors do when a tile of A is put on a square: the shadows of the square, the ones above and below it, to its right and to its upper and lower right are cleared and worked out again from
    // the walls alone, so a shadow drawn by hand on one of them is gone. (Also done when a tile that is not of A takes a wall away: then the walls round the squares are not what they were.)
    internal static void TilePlaced(IMap map, int x, int y)
    {
        foreach (var (dx, dy) in RecalculatedSquares)
        {
            var column = x + dx;
            var row = y + dy;
            if (column < 0 || row < 0 || column >= map.Width || row >= map.Height)
                continue;

            var wanted = map.WallShadowAt(column, row);
            if (map.GetShadowFlags(column, row) != wanted)
                map.SetShadow(column, row, wanted);
        }
    }

    private static readonly (int, int)[] RecalculatedSquares = [(0, 0), (0, -1), (0, 1), (1, 0), (1, -1), (1, 1)];

    internal static bool IsTileOfA(int tileId) => tileId > 0 && tileId < TileIds.Max && (TileIds.IsAutotile(tileId) || TileIds.IsA5(tileId));
}
