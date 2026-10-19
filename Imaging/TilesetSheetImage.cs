using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Storages.Abstraction;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace RPGMDataKit.Imaging;

/// <summary>A block of an autotile sheet (A1-A4): the picture of one kind of autotile, in tiles of the sheet.</summary>
/// <param name="Kind">The kind (0-127, counted from A1); the id of its first shape is <c>TileIds.MakeAutotile(Kind, 0)</c>.</param>
/// <param name="Column">The column of its left edge, in tiles.</param>
/// <param name="Row">The row of its top edge, in tiles.</param>
/// <param name="Width">The width in tiles.</param>
/// <param name="Height">The height in tiles.</param>
public readonly record struct AutotileBlock(int Kind, int Column, int Row, int Width, int Height);

public sealed class TilesetSheetImageOptions
{
    /// <summary>Write the numbers: the id of every tile of B-E and A5, the kind and the id of the first shape of every block of A1-A4. On by default.</summary>
    public bool Labels { get; init; } = true;

    /// <summary>Draw the lines round the tiles (B-E, A5) or the blocks (A1-A4). On by default.</summary>
    public bool Grid { get; init; } = true;

    /// <summary>The light line of the frame of a tile or block, on its edge: white, nearly whole (alpha 230).</summary>
    public Rgba32 GridColor { get; init; } = new(255, 255, 255, 230);

    /// <summary>The dark line just inside of the light one (so that the frame is seen on a light picture too): black, nearly whole (alpha 230). Transparent for none.</summary>
    public Rgba32 GridShadowColor { get; init; } = new(0, 0, 0, 230);

    /// <summary>The width of each of the two lines in pixels. 1 by default; the frames of two neighbours give a light band of twice that between two dark ones.</summary>
    public int GridLineWidth { get; init; } = 1;

    /// <summary>The colour under the sheet, so that the transparent parts are seen; grey by default.</summary>
    public Rgba32 Background { get; init; } = new(96, 96, 96, 255);

    /// <summary>The font of the numbers; a font of the system when not given (see <see cref="MapGridOptions.FontFamily"/>).</summary>
    public FontFamily? FontFamily { get; init; }
}

/// <summary>
/// A sheet of a tileset (A1-E) as a picture with the numbers of its tiles written on it: to find the id of a tile to paint with (<c>PaintTile</c>), or the kind of an autotile.
/// On B, C, D, E and A5 every tile has its id; on A1-A4 every block (one kind of autotile) has its kind and the id of its first shape.
/// </summary>
public static class TilesetSheetImage
{
    private const int NormalTileColumns = 16;
    private const float LabelSize = 0.28f;
    private const int Padding = 2;
    private static readonly (int, int)[] Offsets = [(-1, -1), (0, -1), (1, -1), (-1, 0), (1, 0), (-1, 1), (0, 1), (1, 1)];

    /// <summary>
    /// The id of the tile at the column and row of a sheet of normal tiles (B, C, D, E, A5), in tiles. A sheet is two halves side by side, the ids of the left half going down its rows of 8
    /// (0-127 on B) and the right half going on (128-255). A5 has 8 columns (1536-1663).
    /// </summary>
    /// <exception cref="ArgumentException">The sheet is one of autotiles (A1-A4).</exception>
    public static int NormalTileId(TileSheet sheet, int column, int row)
    {
        var first = sheet switch
        {
            TileSheet.B => TileIds.B,
            TileSheet.C => TileIds.C,
            TileSheet.D => TileIds.D,
            TileSheet.E => TileIds.E,
            TileSheet.A5 => TileIds.A5,
            _ => throw new ArgumentException($"{sheet} is a sheet of autotiles: it has blocks, not tiles of its own", nameof(sheet)),
        };

        return first + column / 8 * 128 + row * 8 + column % 8;
    }

    /// <summary>The blocks of a sheet of autotiles (A1-A4) where the engine takes them from (<c>Tilemap._drawAutotile</c>): the kind and the place of each. The water of A1 has three pictures side by side (the frames); the first is the block.</summary>
    /// <exception cref="ArgumentException">The sheet is not A1-A4.</exception>
    public static IReadOnlyList<AutotileBlock> Blocks(TileSheet sheet)
    {
        var blocks = new List<AutotileBlock>();
        switch (sheet)
        {
            case TileSheet.A1:
                for (var kind = 0; kind < 16; kind++)
                    blocks.Add(A1Block(kind));
                break;
            case TileSheet.A2:
                for (var kind = 16; kind < 48; kind++)
                    blocks.Add(new AutotileBlock(kind, kind % 8 * 2, (kind / 8 - 2) * 3, 2, 3));
                break;
            case TileSheet.A3:
                for (var kind = 48; kind < 80; kind++)
                    blocks.Add(new AutotileBlock(kind, kind % 8 * 2, (kind / 8 - 6) * 2, 2, 2));
                break;
            case TileSheet.A4:
                for (var kind = 80; kind < 128; kind++)
                {
                    var ty = kind / 8;
                    blocks.Add(new AutotileBlock(kind, kind % 8 * 2, (int)Math.Floor((ty - 10) * 2.5 + (ty % 2 == 1 ? 0.5 : 0)), 2, ty % 2 == 1 ? 2 : 3));
                }

                break;
            default:
                throw new ArgumentException($"{sheet} is a sheet of normal tiles: it has tiles, not blocks", nameof(sheet));
        }

        return blocks;
    }

    private static AutotileBlock A1Block(int kind)
    {
        // the sea and the deep sea, then the two still waters, then the water (even) and the waterfalls (odd) of the rest
        if (kind < 4)
            return new AutotileBlock(kind, kind < 2 ? 0 : 6, kind % 2 * 3, 2, 3);

        var tx = kind % 8;
        var ty = kind / 8;
        var column = tx / 4 * 8 + (kind % 2 == 1 ? 6 : 0);
        var row = ty * 6 + tx / 2 % 2 * 3;

        return new AutotileBlock(kind, column, row, 2, 3);
    }

    /// <summary>The picture of the sheet with the numbers of its tiles (or blocks) and the lines round them, as set in the options.</summary>
    /// <exception cref="InvalidOperationException">The numbers are asked for and there is no font in the system (give <see cref="TilesetSheetImageOptions.FontFamily"/>).</exception>
    public static Image<Rgba32> Render(TilesetSheet pixels, TileSheet sheet, TilesetSheetImageOptions? options = null)
    {
        options ??= new TilesetSheetImageOptions();
        var size = pixels.TileSize(sheet);
        if (options.Grid && (options.GridLineWidth < 1 || options.GridLineWidth > size / 4))
            throw new ArgumentOutOfRangeException(nameof(options), options.GridLineWidth, $"The width of a line of the grid is from 1 to a quarter of a tile ({size / 4})");

        var canvas = new PixelCanvas(pixels.Width, pixels.Height, options.Background);
        canvas.Draw(pixels, 0, 0, pixels.Width, pixels.Height, 0, 0);

        var cells = Cells(pixels, sheet, size);
        if (options.Grid)
            DrawGrid(canvas, cells, size, options);

        var image = canvas.ToImage();
        if (options.Labels)
            DrawLabels(image, cells, size, options.FontFamily ?? GridPainter.DefaultFamily());

        return image;
    }

    /// <summary>The picture of a sheet of a tileset of the project (see <see cref="Render(TilesetSheet, TileSheet, TilesetSheetImageOptions?)"/>).</summary>
    /// <exception cref="ArgumentException">There is no such tileset.</exception>
    /// <exception cref="InvalidOperationException">The tileset has no picture for the sheet (no name, or no file).</exception>
    public static Image<Rgba32> RenderTilesetSheet(this Storage storage, int tilesetId, TileSheet sheet, TilesetSheetImageOptions? options = null)
    {
        var tileset = storage.Tilesets.Get(tilesetId) ?? throw new ArgumentException($"There is no tileset {tilesetId}", nameof(tilesetId));
        var pixels = new StorageTilesetSheets(storage, tileset).Get(sheet)
                     ?? throw new InvalidOperationException($"The tileset {tilesetId} has no picture for {sheet} (no name, or no file in the folder of the pictures)");

        return Render(pixels, sheet, options);
    }

    // what gets a number: a rectangle in tiles and its text
    private readonly record struct Cell(int Column, int Row, int Width, int Height, string Title, string? Subtitle);

    private static List<Cell> Cells(TilesetSheet pixels, TileSheet sheet, int size)
    {
        var cells = new List<Cell>();
        var columns = pixels.Width / size;
        var rows = pixels.Height / size;
        if (sheet is TileSheet.A1 or TileSheet.A2 or TileSheet.A3 or TileSheet.A4)
        {
            foreach (var block in Blocks(sheet))
            {
                if (block.Column + block.Width <= columns && block.Row + block.Height <= rows)
                    cells.Add(new Cell(block.Column, block.Row, block.Width, block.Height, $"kind {block.Kind}", $"id {TileIds.MakeAutotile(block.Kind, 0)}"));
            }
        }
        else
        {
            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                    cells.Add(new Cell(column, row, 1, 1, NormalTileId(sheet, column, row).ToString(), null));
            }
        }

        return cells;
    }

    // a frame round every tile or block: a light ring on its edge and a dark one just inside of it, so it is seen on a light picture and on a dark one (two neighbours give a light band between two dark ones)
    private static void DrawGrid(PixelCanvas canvas, List<Cell> cells, int size, TilesetSheetImageOptions options)
    {
        var t = options.GridLineWidth;
        foreach (var cell in cells)
        {
            var x = cell.Column * size;
            var y = cell.Row * size;
            var width = cell.Width * size;
            var height = cell.Height * size;
            Ring(canvas, x, y, width, height, t, options.GridColor);
            Ring(canvas, x + t, y + t, width - 2 * t, height - 2 * t, t, options.GridShadowColor);
        }
    }

    private static void Ring(PixelCanvas canvas, int x, int y, int width, int height, int t, Rgba32 color)
    {
        if (width < 2 * t || height < 2 * t)
            return;

        canvas.Fill(x, y, width, t, color);
        canvas.Fill(x, y + height - t, width, t, color);
        canvas.Fill(x, y + t, t, height - 2 * t, color);
        canvas.Fill(x + width - t, y + t, t, height - 2 * t, color);
    }

    private static void DrawLabels(Image<Rgba32> image, List<Cell> cells, int size, FontFamily family)
    {
        var baseFont = family.CreateFont(Math.Max(4f, size * LabelSize));
        var white = Color.White;
        var outline = Color.FromPixel(new Rgba32(0, 0, 0, 220));
        image.Mutate(context =>
        {
            foreach (var cell in cells)
            {
                var width = cell.Width * size - 2 * Padding;
                var x = cell.Column * size + Padding;
                var y = cell.Row * size + Padding;
                foreach (var text in new[] { cell.Title, cell.Subtitle })
                {
                    if (text == null)
                        continue;

                    var font = Fit(baseFont, text, width);
                    Write(context, text, font, new PointF(x, y), white, outline);
                    y += (int)Math.Ceiling(font.Size * 1.15f);
                }
            }
        });
    }

    private static void Write(IImageProcessingContext context, string text, Font font, PointF origin, Color color, Color outline)
    {
        foreach (var (dx, dy) in Offsets)
            context.DrawText(new RichTextOptions(font) { Origin = new PointF(origin.X + dx, origin.Y + dy) }, text, outline);

        context.DrawText(new RichTextOptions(font) { Origin = origin }, text, color);
    }

    private static Font Fit(Font font, string text, float width)
    {
        var measured = TextMeasurer.MeasureSize(text, new TextOptions(font)).Width;

        return measured <= width || measured <= 0 ? font : new Font(font, Math.Max(4f, font.Size * width / measured));
    }
}
