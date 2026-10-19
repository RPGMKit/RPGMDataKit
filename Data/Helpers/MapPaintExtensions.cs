using RPGMDataKit.Data.Structures.Interfaces;

namespace RPGMDataKit.Data.Helpers;

/// <summary>
/// Painting a tile the way the editors do with the layer set to automatic: the layer is chosen from the tile, and what is on the square is moved or replaced.
/// <para>
/// <b>MV</b> (four layers of tiles): a tile of the ground (A1, the left half of A2, A3, A4, A5) goes to the layer 0 and <b>everything else on the square is removed</b> (the overlay, the tiles of B-E);
/// an overlay (the right half of A2, the deep sea, see <see cref="TileIds.IsOverlay"/>) goes to the layer 1 in place of the overlay that was there, the ground stays and <b>the tiles of B-E are removed</b>
/// (a tile of A removes everything above its layer); a tile of B-E goes to the top layer,
/// the one that was on the top moves down one layer, and the one that was on the layer below it is lost (two tiles of B-E fit on a square); the same tile as the one on top is not added a second time.
/// <b>VX Ace</b> (three layers): the same for the ground and the overlays; B-E has one layer, the top one, and a tile painted there replaces the one that was there.
/// </para>
/// <para>
/// MV was found by painting in the editor (1.6.x) with the layer on Auto; VX Ace behaves the same with one layer for B-E (the maintainer says so, and the layers fit a saved game of that editor,
/// see <c>.claude-docs/maps-and-tiles.md</c>). The autotiles round the square are reshaped afterwards (<see cref="MapAutotileExtensions"/>). The shadows beside walls are kept by <see cref="MapTileExtensions.SetTile"/> (see <see cref="MapShadowExtensions"/>).
/// </para>
/// </summary>
public static class MapPaintExtensions
{
    /// <summary>
    /// Puts a tile on the square on the layer the editors choose for it (see <see cref="MapPaintExtensions"/>), and reshapes the autotiles round it.
    /// Use <see cref="MapAutotileExtensions.PaintTile"/> to paint on a layer you choose.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The square is outside of the map, or the id is not that of a tile (0, an unused id, or past the last one).</exception>
    public static void PaintTileAuto(this IMap map, int x, int y, int tileId)
    {
        var sheet = TileIds.SheetOf(tileId) ?? throw new ArgumentOutOfRangeException(nameof(tileId), tileId, "Not the id of a tile (0, an unused id, or past the last one)");
        map.GetTile(x, y, 0);   // the square is checked here

        var layers = map.TileLayerCount();
        if (sheet is TileSheet.B or TileSheet.C or TileSheet.D or TileSheet.E)
        {
            var top = layers - 1;
            var onTop = map.GetTile(x, y, top);
            if (layers >= 4 && onTop != 0 && onTop != tileId)
                map.SetTile(x, y, top - 1, onTop);

            map.SetTile(x, y, top, tileId);
        }
        else if (TileIds.IsOverlay(tileId))
        {
            map.SetTile(x, y, 1, tileId);
            for (var layer = 2; layer < layers; layer++)
                map.SetTile(x, y, layer, 0);
        }
        else
        {
            map.SetTile(x, y, 0, tileId);
            for (var layer = 1; layer < layers; layer++)
                map.SetTile(x, y, layer, 0);
        }

        for (var row = Math.Max(0, y - 1); row <= Math.Min(map.Height - 1, y + 1); row++)
        {
            for (var column = Math.Max(0, x - 1); column <= Math.Min(map.Width - 1, x + 1); column++)
            {
                for (var layer = 0; layer < layers; layer++)
                    map.UpdateAutotile(column, row, layer);
            }
        }
    }
}
