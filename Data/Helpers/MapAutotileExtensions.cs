using RPGMDataKit.Data.Structures.Interfaces;

namespace RPGMDataKit.Data.Helpers;

/// <summary>
/// Choosing the shape of the autotiles of a map from what is round them, as the editors do when a tile is painted (the engines only read the shape from the tile id).
/// <para>
/// <b>What is connected.</b> Only the same layer is looked at. A neighbour is connected when it is an autotile of the same kind. Besides that: water (A1, not a waterfall) is connected to a waterfall; the side of an
/// A4 wall is connected, to left and right, to any A4 wall tile (a top or a side of any kind). What is outside of the map is connected for water, ground and waterfalls, and for roofs and walls to left and right only.
/// Everything else (empty, a normal tile, an autotile of another kind) is not.
/// </para>
/// <para>
/// This was found by comparing the rule with the maps that the editors saved (see <c>.claude-docs/maps-and-tiles.md</c>); the editors do not always reshape a tile when something next to it changes,
/// so a saved map has tiles that this rule would give another shape.
/// </para>
/// </summary>
public static class MapAutotileExtensions
{
    /// <summary>The shape (0-47) that the autotile at the square should have, from its neighbours on the same layer.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The square or the layer is outside of the map.</exception>
    /// <exception cref="ArgumentException">The tile there is not an autotile.</exception>
    public static int AutotileShapeAt(this IMap map, int x, int y, int layer)
    {
        var tile = map.GetTile(x, y, layer);
        if (!IsAutotile(tile))
            throw new ArgumentException($"The tile {tile} at {x},{y} on the layer {layer} is not an autotile");

        return ShapeOf(map, x, y, layer, tile);
    }

    /// <summary>Gives the autotile at the square the shape its neighbours ask for. Does nothing to a tile that is not an autotile. Returns whether the tile changed.</summary>
    public static bool UpdateAutotile(this IMap map, int x, int y, int layer)
    {
        var tile = map.GetTile(x, y, layer);
        if (!IsAutotile(tile))
            return false;

        var made = TileIds.MakeAutotile(TileIds.Kind(tile), ShapeOf(map, x, y, layer, tile));
        if (made == tile)
            return false;

        map.SetTile(x, y, layer, made);

        return true;
    }

    /// <summary>Gives every autotile of the map (on every layer of tiles) the shape its neighbours ask for. Returns how many tiles changed.</summary>
    public static int UpdateAutotiles(this IMap map) => map.UpdateAutotiles(0, 0, map.Width, map.Height);

    /// <summary>Gives the autotiles of a part of the map (x, y and the size in squares) the shape their neighbours ask for. Returns how many tiles changed.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The part is not inside of the map.</exception>
    public static int UpdateAutotiles(this IMap map, int x, int y, int width, int height)
    {
        if (width < 0 || height < 0 || x < 0 || y < 0 || x + width > map.Width || y + height > map.Height)
            throw new ArgumentOutOfRangeException(nameof(width), $"The part {x},{y} {width}x{height} is not inside of the map ({map.Width}x{map.Height})");

        var changed = 0;
        for (var layer = 0; layer < map.TileLayerCount(); layer++)
        {
            for (var row = y; row < y + height; row++)
            {
                for (var column = x; column < x + width; column++)
                    changed += map.UpdateAutotile(column, row, layer) ? 1 : 0;
            }
        }

        return changed;
    }

    /// <summary>
    /// Puts a tile on the layer as the editors do: an autotile gets the shape its neighbours ask for (the shape in the id is not used, only the kind), and the autotiles round the square, on the same layer,
    /// are reshaped to it. A tile that is not an autotile (or 0, nothing) is put as it is and the neighbours are reshaped to the change. (<see cref="MapTileExtensions.SetTile"/>, which this calls, also keeps the shadows beside walls: <see cref="MapShadowExtensions"/>).
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The square or the layer is outside of the map, or the id is not a tile id.</exception>
    public static void PaintTile(this IMap map, int x, int y, int layer, int tileId)
    {
        map.SetTile(x, y, layer, tileId);

        for (var row = Math.Max(0, y - 1); row <= Math.Min(map.Height - 1, y + 1); row++)
        {
            for (var column = Math.Max(0, x - 1); column <= Math.Min(map.Width - 1, x + 1); column++)
                map.UpdateAutotile(column, row, layer);
        }
    }

    private static bool IsAutotile(int tile) => TileIds.IsAutotile(tile) && tile < TileIds.Max;

    private static int ShapeOf(IMap map, int x, int y, int layer, int tile)
    {
        bool Connected(int dx, int dy) => AreConnected(map, x, y, layer, tile, dx, dy);

        if (TileIds.IsWaterfall(tile))
            return AutotileShapes.Waterfall(Connected(-1, 0), Connected(1, 0));
        if (TileIds.IsFloorTypeAutotile(tile))
        {
            return AutotileShapes.Floor(Connected(0, -1), Connected(0, 1), Connected(-1, 0), Connected(1, 0),
                Connected(-1, -1), Connected(1, -1), Connected(-1, 1), Connected(1, 1));
        }

        return AutotileShapes.Wall(Connected(0, -1), Connected(0, 1), Connected(-1, 0), Connected(1, 0));
    }

    private static bool AreConnected(IMap map, int x, int y, int layer, int tile, int dx, int dy)
    {
        var nx = x + dx;
        var ny = y + dy;
        if (nx < 0 || ny < 0 || nx >= map.Width || ny >= map.Height)
            return !TileIds.IsWallTypeAutotile(tile) || (dy == 0 && MapTileExtensions.IsVxAceLayout(map));

        var other = map.Data[nx, ny, layer];
        if (!IsAutotile(other))
            return false;
        if (TileIds.Kind(tile) == TileIds.Kind(other))
            return true;
        if (TileIds.IsWater(tile) && !TileIds.IsWaterfall(tile) && TileIds.IsWaterfall(other))
            return true;

        return dy == 0 && TileIds.IsWallSide(tile) && TileIds.IsWallTop(other);
    }
}
