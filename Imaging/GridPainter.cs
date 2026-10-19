using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace RPGMDataKit.Imaging;

/// <summary>The texts of the squares of a grid (the lines are drawn by <see cref="PixelCanvas.DrawGrid"/>).</summary>
internal static class GridPainter
{
    private static readonly string[] PreferredFonts = ["Arial", "Segoe UI", "DejaVu Sans", "Liberation Sans"];

    // the size of the text as a part of the tile, and the space kept around it in the square, in pixels
    private const float TextSize = 0.28f;
    private const float MinTextSize = 4f;
    private const int Padding = 2;

    private static readonly (int, int)[] Offsets = [(-1, -1), (0, -1), (1, -1), (-1, 0), (1, 0), (-1, 1), (0, 1), (1, 1)];

    public static void CheckLineWidth(MapGridOptions grid, int tileSize)
    {
        if (grid.LineWidth < 1 || grid.LineWidth > tileSize / 4)
            throw new ArgumentOutOfRangeException(nameof(grid), grid.LineWidth, $"The width of a line is from 1 to a quarter of a tile ({tileSize / 4})");
    }

    /// <summary>Writes the texts of the squares of the part of the map that the picture shows.</summary>
    public static void DrawLabels(Image<Rgba32> image, IMap map, MapRegion region, int size, MapGridOptions grid)
    {
        var family = grid.FontFamily ?? DefaultFamily();
        var baseFont = family.CreateFont(Math.Max(MinTextSize, size * TextSize));
        var texts = new List<(int Column, int Row, string Text)>();
        for (var y = 0; y < region.Height; y++)
        {
            for (var x = 0; x < region.Width; x++)
            {
                if (TextOf(map, region.X + x, region.Y + y, grid) is { Length: > 0 } text)
                    texts.Add((x, y, text));
            }
        }

        if (texts.Count == 0)
            return;

        image.Mutate(context =>
        {
            foreach (var (column, row, text) in texts)
            {
                var font = Fit(baseFont, text, size - 2 * Padding);
                var left = column * size;
                var top = row * size;
                var center = grid.LabelPosition == GridLabelPosition.Center;
                var origin = center ? new PointF(left + size / 2f, top + size / 2f) : new PointF(left + Padding, top + Padding);
                var options = new RichTextOptions(font)
                {
                    Origin = origin,
                    HorizontalAlignment = center ? HorizontalAlignment.Center : HorizontalAlignment.Left,
                    VerticalAlignment = center ? VerticalAlignment.Center : VerticalAlignment.Top,
                };

                if (grid.LabelOutline.A != 0)
                {
                    // the outline: the text in the outline colour, moved one pixel in each of the eight directions
                    var outline = Color.FromPixel(grid.LabelOutline);
                    foreach (var (ox, oy) in Offsets)
                    {
                        var moved = new RichTextOptions(options) { Origin = new PointF(origin.X + ox, origin.Y + oy) };
                        context.DrawText(moved, text, outline);
                    }
                }

                context.DrawText(options, text, Color.FromPixel(grid.LabelColor));
            }
        });
    }

    private static string? TextOf(IMap map, int x, int y, MapGridOptions grid)
    {
        if (grid.Label != null)
            return grid.Label(x, y);

        switch (grid.Labels)
        {
            case GridLabels.Coordinates:
                return $"{x},{y}";
            case GridLabels.Region:
                return map.GetRegion(x, y) is var region and > 0 ? region.ToString() : null;
            case GridLabels.TileId:
                var layer = grid.TileLayer;
                return layer >= 0 && layer < map.TileLayerCount() && map.GetTile(x, y, layer) is var tile and > 0 ? tile.ToString() : null;
            default:
                return null;
        }
    }

    // a text that is wider than the square is made smaller to fit
    private static Font Fit(Font font, string text, float width)
    {
        var measured = TextMeasurer.MeasureSize(text, new TextOptions(font)).Width;
        if (measured <= width || measured <= 0)
            return font;

        return new Font(font, Math.Max(MinTextSize, font.Size * width / measured));
    }

    private static FontFamily DefaultFamily()
    {
        foreach (var name in PreferredFonts)
        {
            if (SystemFonts.TryGet(name, out var family))
                return family;
        }

        foreach (var family in SystemFonts.Families)
            return family;

        throw new InvalidOperationException("No font to write the texts of the grid with: the system has none, give one in MapGridOptions.FontFamily");
    }
}
