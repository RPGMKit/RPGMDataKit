using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RPGMDataKit.Imaging;

/// <summary>
/// A picture of the tiles of a map, put together the way the engine of MV draws them (<c>Tilemap</c>): the squares one by one, each with its lower tiles, shadow, edge of a table and upper tiles,
/// the autotiles cut as four quarters of their picture. VX Ace draws the same tiles the same way. The events and the characters are not in the picture.
/// <para>
/// The result is checked against the code of the engine on every kind of tile (see <c>MapRendererTests</c>).
/// </para>
/// </summary>
public static class MapRenderer
{
    private static readonly int[] WaterSurface = [0, 1, 2, 1];

    // the colour of a shadow of the engine: rgba(0, 0, 0, 0.5)
    private static readonly Rgba32 ShadowColor = new(0, 0, 0, 128);

    /// <summary>Draws the map (or a part of it) with the tiles of the tileset.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The part is not inside of the map, or the tile size is not a multiple of 4.</exception>
    /// <exception cref="InvalidOperationException">No size of the tile is known (the tileset has no sheet and the options do not say), or the sheets disagree about it.</exception>
    public static Image<Rgba32> Render(IMap map, ITileSet tileset, ITilesetSheets sheets, MapImageOptions? options = null)
    {
        options ??= new MapImageOptions();
        var region = options.Region ?? new MapRegion(0, 0, map.Width, map.Height);
        CheckRegion(map, region);

        var size = options.TileSize ?? DetectTileSize(sheets);
        if (size < 4 || size % 4 != 0)
            throw new ArgumentOutOfRangeException(nameof(options), size, "The tile size must be a multiple of 4");

        if (options.Grid is { } grid)
            GridPainter.CheckLineWidth(grid, size);

        var canvas = new PixelCanvas(region.Width * size, region.Height * size, options.Background);
        var drawer = new Drawer(map, tileset, sheets, size, options.AnimationFrame, canvas);

        if (options.Layers.HasFlag(MapImageLayers.Lower))
        {
            for (var y = region.Y; y < region.Y + region.Height; y++)
            {
                for (var x = region.X; x < region.X + region.Width; x++)
                    drawer.DrawLower(x, y, (x - region.X) * size, (y - region.Y) * size, options.Shadows);
            }
        }

        if (options.Layers.HasFlag(MapImageLayers.Upper))
        {
            for (var y = region.Y; y < region.Y + region.Height; y++)
            {
                for (var x = region.X; x < region.X + region.Width; x++)
                    drawer.DrawUpper(x, y, (x - region.X) * size, (y - region.Y) * size);
            }
        }

        if (options.Grid is not { } gridOptions)
            return canvas.ToImage();

        canvas.DrawGrid(size, gridOptions.LineWidth, gridOptions.Color, gridOptions.ShadowColor);
        var image = canvas.ToImage();
        if (gridOptions.HasLabels)
            GridPainter.DrawLabels(image, map, region, size, gridOptions);

        return image;
    }

    private static void CheckRegion(IMap map, MapRegion region)
    {
        if (region.Width < 1 || region.Height < 1)
            throw new ArgumentOutOfRangeException(nameof(region), region, "The part of the map is at least one square wide and high");
        if (region.X < 0 || region.Y < 0 || region.X + region.Width > map.Width || region.Y + region.Height > map.Height)
            throw new ArgumentOutOfRangeException(nameof(region), region, $"The part of the map must be inside of the map ({map.Width} x {map.Height} squares)");
    }

    // the size of a tile from the sheets: they all have to give the same
    private static int DetectTileSize(ITilesetSheets sheets)
    {
        var sizes = new HashSet<int>();
        foreach (var sheet in Enum.GetValues<TileSheet>())
        {
            if (sheets.Get(sheet) is { } pixels)
                sizes.Add(pixels.TileSize(sheet));
        }

        if (sizes.Count == 0)
            throw new InvalidOperationException("The tileset has no sheet to take the size of a tile from: give it in the options (TileSize)");
        if (sizes.Count > 1)
            throw new InvalidOperationException($"The sheets of the tileset have different sizes of a tile ({string.Join(", ", sizes.Order())})");

        return sizes.Single();
    }

    // the drawing of the squares: the same steps as Tilemap._paintTiles, _drawTile, _drawNormalTile, _drawAutotile, _drawTableEdge and _drawShadow of MV
    private sealed class Drawer(IMap map, ITileSet tileset, ITilesetSheets sheets, int size, int frame, PixelCanvas canvas)
    {
        private readonly int _quarter = size / 2;

        private int Tile(int x, int y, int layer) =>
            (uint)x < (uint)map.Width && (uint)y < (uint)map.Height && layer < map.TileLayerCount() ? map.Data[x, y, layer] : 0;

        // the star: the tile is drawn over the characters
        private bool IsHigher(int tileId) => (tileset.GetFlags(tileId) & (int)TileFlags.IsHigherTile) != 0;

        // a table: a tile of A2 with the counter flag; the edge of it is drawn on the square below
        private bool IsTable(int tileId) => TileIds.IsA2(tileId) && (tileset.GetFlags(tileId) & (int)TileFlags.Counter) != 0;

        public void DrawLower(int x, int y, int dx, int dy, bool shadows)
        {
            int tile0 = Tile(x, y, 0), tile1 = Tile(x, y, 1), tile2 = Tile(x, y, 2), tile3 = Tile(x, y, 3);
            var shadow = y < map.Height && x < map.Width ? map.GetShadow(x, y) : 0;
            var above = Tile(x, y - 1, 1);

            if (!IsHigher(tile0))
                DrawTile(tile0, dx, dy);
            if (!IsHigher(tile1))
                DrawTile(tile1, dx, dy);

            if (shadows)
                DrawShadow(shadow, dx, dy);

            if (IsTable(above) && !IsTable(tile1) && !TileIds.IsShadowing(tile0))
                DrawTableEdge(above, dx, dy);

            if (!IsHigher(tile2))
                DrawTile(tile2, dx, dy);
            if (!IsHigher(tile3))
                DrawTile(tile3, dx, dy);
        }

        public void DrawUpper(int x, int y, int dx, int dy)
        {
            for (var layer = 0; layer < 4; layer++)
            {
                var tileId = Tile(x, y, layer);
                if (IsHigher(tileId))
                    DrawTile(tileId, dx, dy);
            }
        }

        private void DrawTile(int tileId, int dx, int dy)
        {
            if (!TileIds.IsVisible(tileId))
                return;

            if (TileIds.IsAutotile(tileId))
                DrawAutotile(tileId, dx, dy);
            else
                DrawNormalTile(tileId, dx, dy);
        }

        private void DrawNormalTile(int tileId, int dx, int dy)
        {
            var sheet = TileIds.IsA5(tileId) ? TileSheet.A5 : (TileSheet)(5 + tileId / 256);
            if (!Enum.IsDefined(sheet) || sheets.Get(sheet) is not { } source)
                return;

            var (column, row) = TileIds.NormalTileCell(tileId);
            canvas.Draw(source, column * size, row * size, size, size, dx, dy);
        }

        private void DrawAutotile(int tileId, int dx, int dy)
        {
            var table = AutotileTables.Floor;
            var kind = TileIds.Kind(tileId);
            var shape = TileIds.Shape(tileId);
            var tx = kind % 8;
            var ty = kind / 8;
            int bx = 0, by = 0;
            TileSheet sheet;
            var isTable = false;

            if (TileIds.IsA1(tileId))
            {
                var water = WaterSurface[Mod(frame, 4)];
                sheet = TileSheet.A1;
                if (kind == 0)
                {
                    bx = water * 2;
                    by = 0;
                }
                else if (kind == 1)
                {
                    bx = water * 2;
                    by = 3;
                }
                else if (kind == 2)
                {
                    bx = 6;
                    by = 0;
                }
                else if (kind == 3)
                {
                    bx = 6;
                    by = 3;
                }
                else
                {
                    bx = tx / 4 * 8;
                    by = ty * 6 + tx / 2 % 2 * 3;
                    if (kind % 2 == 0)
                    {
                        bx += water * 2;
                    }
                    else
                    {
                        bx += 6;
                        table = AutotileTables.Waterfall;
                        by += Mod(frame, 3);
                    }
                }
            }
            else if (TileIds.IsA2(tileId))
            {
                sheet = TileSheet.A2;
                bx = tx * 2;
                by = (ty - 2) * 3;
                isTable = IsTable(tileId);
            }
            else if (TileIds.IsA3(tileId))
            {
                sheet = TileSheet.A3;
                bx = tx * 2;
                by = (ty - 6) * 2;
                table = AutotileTables.Wall;
            }
            else
            {
                sheet = TileSheet.A4;
                bx = tx * 2;
                by = (int)Math.Floor((ty - 10) * 2.5 + (ty % 2 == 1 ? 0.5 : 0));
                if (ty % 2 == 1)
                    table = AutotileTables.Wall;
            }

            if (shape >= table.Length || sheets.Get(sheet) is not { } source)
                return;

            var quarters = table[shape];
            var half = _quarter / 2;
            for (var i = 0; i < 4; i++)
            {
                var (qsx, qsy) = quarters[i];
                var sx = (bx * 2 + qsx) * _quarter;
                var sy = (by * 2 + qsy) * _quarter;
                var qx = dx + i % 2 * _quarter;
                var qy = dy + i / 2 * _quarter;

                if (isTable && (qsy == 1 || qsy == 5))
                {
                    // the front of a table: the quarter of the top of the table below it, and the upper half of this quarter under that
                    var otherX = qsy == 1 ? new[] { 0, 3, 2, 1 }[qsx] : qsx;
                    var other = (bx * 2 + otherX) * _quarter;
                    var otherY = (by * 2 + 3) * _quarter;
                    canvas.Draw(source, other, otherY, _quarter, _quarter, qx, qy);
                    canvas.Draw(source, sx, sy, _quarter, half, qx, qy + half);
                }
                else
                {
                    canvas.Draw(source, sx, sy, _quarter, _quarter, qx, qy);
                }
            }
        }

        // the lower half of the front of a table whose top is on the square above
        private void DrawTableEdge(int tileId, int dx, int dy)
        {
            if (!TileIds.IsA2(tileId) || sheets.Get(TileSheet.A2) is not { } source)
                return;

            var kind = TileIds.Kind(tileId);
            var shape = TileIds.Shape(tileId);
            var bx = kind % 8 * 2;
            var by = (kind / 8 - 2) * 3;
            if (shape >= AutotileTables.Floor.Length)
                return;

            var table = AutotileTables.Floor[shape];
            var half = _quarter / 2;
            for (var i = 0; i < 2; i++)
            {
                var (qsx, qsy) = table[2 + i];
                var sx = (bx * 2 + qsx) * _quarter;
                var sy = (by * 2 + qsy) * _quarter + half;
                canvas.Draw(source, sx, sy, _quarter, half, dx + i % 2 * _quarter, dy);
            }
        }

        private void DrawShadow(int shadow, int dx, int dy)
        {
            for (var i = 0; i < 4; i++)
            {
                if ((shadow & (1 << i)) != 0)
                    canvas.Fill(dx + i % 2 * _quarter, dy + i / 2 * _quarter, _quarter, _quarter, ShadowColor);
            }
        }

        private static int Mod(int value, int divisor) => (value % divisor + divisor) % divisor;
    }
}
