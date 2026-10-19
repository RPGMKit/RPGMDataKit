using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>A sheet of a tileset with the numbers of its tiles (<see cref="TilesetSheetImage"/>).</summary>
public sealed class TilesetSheetImageTests
{
    private const int Tile = 8;

    // a sheet of the size the engine reads: B-E and A1-A4 are 16 tiles wide, A5 is 8 wide; all of it clear, but for the tile at the top left
    private static TilesetSheet Sheet(TileSheet sheet, int rows = 16)
    {
        var columns = sheet == TileSheet.A5 ? 8 : 16;
        var pixels = new Rgba32[columns * Tile * rows * Tile];
        pixels[0] = new Rgba32(10, 200, 10, 255);

        return new TilesetSheet(columns * Tile, rows * Tile, pixels);
    }

    private static Rgba32[] Pixels(Image<Rgba32> image)
    {
        var pixels = new Rgba32[image.Width * image.Height];
        image.CopyPixelDataTo(pixels);

        return pixels;
    }

    // ---- the ids ----

    [Theory]
    [InlineData(TileSheet.B, 0, 0, 0)]
    [InlineData(TileSheet.B, 7, 15, 127)]
    [InlineData(TileSheet.B, 8, 0, 128)]
    [InlineData(TileSheet.B, 5, 11, 93)]
    [InlineData(TileSheet.B, 15, 15, 255)]
    [InlineData(TileSheet.C, 0, 0, 256)]
    [InlineData(TileSheet.D, 3, 1, 523)]
    [InlineData(TileSheet.E, 0, 0, 768)]
    [InlineData(TileSheet.E, 15, 15, 1023)]
    [InlineData(TileSheet.A5, 0, 0, 1536)]
    [InlineData(TileSheet.A5, 3, 2, 1555)]
    [InlineData(TileSheet.A5, 7, 15, 1663)]
    public void TheIdOfATileIsWhereTheEngineTakesItFrom(TileSheet sheet, int column, int row, int id)
    {
        Assert.Equal(id, TilesetSheetImage.NormalTileId(sheet, column, row));
        // the other way: the cell of the id, by the function that the renderer uses
        Assert.Equal((column, row), TileIds.NormalTileCell(id));
    }

    [Theory]
    [InlineData(TileSheet.A1)]
    [InlineData(TileSheet.A4)]
    public void ASheetOfAutotilesHasNoIdsOfTiles(TileSheet sheet)
    {
        Assert.Throws<ArgumentException>(() => TilesetSheetImage.NormalTileId(sheet, 0, 0));
    }

    // ---- the blocks of autotiles ----

    [Theory]
    [InlineData(TileSheet.A1, 16)]
    [InlineData(TileSheet.A2, 32)]
    [InlineData(TileSheet.A3, 32)]
    [InlineData(TileSheet.A4, 48)]
    public void EverySheetOfAutotilesHasABlockForEveryKind(TileSheet sheet, int count)
    {
        var blocks = TilesetSheetImage.Blocks(sheet);

        Assert.Equal(count, blocks.Count);
        Assert.Equal(blocks.Count, blocks.Select(b => b.Kind).Distinct().Count());
        Assert.All(blocks, b => Assert.True(TileIds.Kind(TileIds.MakeAutotile(b.Kind, 0)) == b.Kind));
    }

    [Theory]
    [InlineData(TileSheet.A1, 0, 0, 0, 2, 3)]
    [InlineData(TileSheet.A1, 1, 0, 3, 2, 3)]
    [InlineData(TileSheet.A1, 2, 6, 0, 2, 3)]
    [InlineData(TileSheet.A1, 3, 6, 3, 2, 3)]
    [InlineData(TileSheet.A1, 4, 8, 0, 2, 3)]
    [InlineData(TileSheet.A1, 5, 14, 0, 2, 3)]
    [InlineData(TileSheet.A1, 8, 0, 6, 2, 3)]
    [InlineData(TileSheet.A1, 9, 6, 6, 2, 3)]
    [InlineData(TileSheet.A1, 14, 8, 9, 2, 3)]
    [InlineData(TileSheet.A2, 16, 0, 0, 2, 3)]
    [InlineData(TileSheet.A2, 20, 8, 0, 2, 3)]
    [InlineData(TileSheet.A2, 47, 14, 9, 2, 3)]
    [InlineData(TileSheet.A3, 48, 0, 0, 2, 2)]
    [InlineData(TileSheet.A3, 57, 2, 2, 2, 2)]
    [InlineData(TileSheet.A3, 79, 14, 6, 2, 2)]
    [InlineData(TileSheet.A4, 80, 0, 0, 2, 3)]
    [InlineData(TileSheet.A4, 88, 0, 3, 2, 2)]
    [InlineData(TileSheet.A4, 96, 0, 5, 2, 3)]
    [InlineData(TileSheet.A4, 104, 0, 8, 2, 2)]
    [InlineData(TileSheet.A4, 112, 0, 10, 2, 3)]
    [InlineData(TileSheet.A4, 127, 14, 13, 2, 2)]
    public void ABlockIsWhereTheEngineCutsItFrom(TileSheet sheet, int kind, int column, int row, int width, int height)
    {
        Assert.Equal(new AutotileBlock(kind, column, row, width, height), TilesetSheetImage.Blocks(sheet).Single(b => b.Kind == kind));
    }

    [Theory]
    [InlineData(TileSheet.A1)]
    [InlineData(TileSheet.A2)]
    [InlineData(TileSheet.A3)]
    [InlineData(TileSheet.A4)]
    public void TheBlocksOfASheetDoNotOverlapEachOther(TileSheet sheet)
    {
        var blocks = TilesetSheetImage.Blocks(sheet);
        foreach (var a in blocks)
        {
            foreach (var b in blocks.Where(b => b.Kind > a.Kind))
            {
                var apart = a.Column + a.Width <= b.Column || b.Column + b.Width <= a.Column || a.Row + a.Height <= b.Row || b.Row + b.Height <= a.Row;
                Assert.True(apart, $"the kinds {a.Kind} and {b.Kind} overlap");
            }
        }
    }

    [Theory]
    [InlineData(TileSheet.B)]
    [InlineData(TileSheet.A5)]
    public void ASheetOfNormalTilesHasNoBlocks(TileSheet sheet)
    {
        Assert.Throws<ArgumentException>(() => TilesetSheetImage.Blocks(sheet));
    }

    // ---- the picture ----

    [Theory]
    [InlineData(TileSheet.B)]
    [InlineData(TileSheet.A5)]
    [InlineData(TileSheet.A2)]
    public void WithNothingAskedForThePictureIsTheSheetOverTheBackground(TileSheet which)
    {
        var sheet = Sheet(which);
        var background = new Rgba32(1, 2, 3, 255);

        using var image = TilesetSheetImage.Render(sheet, which, new TilesetSheetImageOptions { Labels = false, Grid = false, Background = background });

        Assert.Equal((sheet.Width, sheet.Height), (image.Width, image.Height));
        var pixels = Pixels(image);
        Assert.Equal(new Rgba32(10, 200, 10, 255), pixels[0]);
        Assert.All(pixels.Skip(1), p => Assert.Equal(background, p));
    }

    [Fact]
    public void TheGridIsDrawnRoundTheTilesOfASheetOfNormalTiles()
    {
        var sheet = Sheet(TileSheet.B);
        var options = new TilesetSheetImageOptions { Labels = false, Background = new Rgba32(0, 0, 0, 255) };

        using var plain = TilesetSheetImage.Render(sheet, TileSheet.B, new TilesetSheetImageOptions { Labels = false, Grid = false, Background = options.Background });
        using var gridded = TilesetSheetImage.Render(sheet, TileSheet.B, options);

        var before = Pixels(plain);
        var after = Pixels(gridded);
        var width = sheet.Width;
        // the squares are told apart on the lines of the tiles, and the middle of a tile is as it was
        Assert.NotEqual(before[Tile * width + Tile], after[Tile * width + Tile]);        // the corner of the tile (1, 1)
        Assert.Equal(before[(Tile + 3) * width + Tile + 3], after[(Tile + 3) * width + Tile + 3]);
    }

    [Fact]
    public void TheFrameIsALightLineOnTheEdgeAndADarkOneInsideOfItAndSeenOnBothLightAndDark()
    {
        var sheet = Sheet(TileSheet.B);
        foreach (var background in new[] { new Rgba32(0, 0, 0, 255), new Rgba32(255, 255, 255, 255) })
        {
            using var gridded = TilesetSheetImage.Render(sheet, TileSheet.B, new TilesetSheetImageOptions { Labels = false, Background = background });
            var pixels = Pixels(gridded);
            var edge = pixels[Tile * sheet.Width + Tile + 3];           // on the top edge of the tile (1, 1)
            var inside = pixels[(Tile + 1) * sheet.Width + Tile + 3];   // one pixel in

            Assert.True(edge.R > 200 && edge.G > 200 && edge.B > 200, $"the edge is not light on {background}: {edge}");
            Assert.True(inside.R < 60 && inside.G < 60 && inside.B < 60, $"the line inside is not dark on {background}: {inside}");
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(Tile / 4 + 1)]
    public void AWidthOfTheLineThatIsNotFromOnePixelToAQuarterOfATileIsRefused(int width)
    {
        var sheet = Sheet(TileSheet.B);

        Assert.Throws<ArgumentOutOfRangeException>(() => TilesetSheetImage.Render(sheet, TileSheet.B, new TilesetSheetImageOptions { Labels = false, GridLineWidth = width }));
    }

    [Fact]
    public void TheGridOfASheetOfAutotilesIsRoundTheBlocksNotTheTiles()
    {
        var sheet = Sheet(TileSheet.A2, 12);
        var background = new Rgba32(0, 0, 0, 255);

        using var plain = TilesetSheetImage.Render(sheet, TileSheet.A2, new TilesetSheetImageOptions { Labels = false, Grid = false, Background = background });
        using var gridded = TilesetSheetImage.Render(sheet, TileSheet.A2, new TilesetSheetImageOptions { Labels = false, Background = background });

        var before = Pixels(plain);
        var after = Pixels(gridded);
        var width = sheet.Width;
        // a block of A2 is 2 tiles wide and 3 high: there is a line at its left (x = 2 tiles) and none at the line of a tile inside of a block (x = 1 tile)
        Assert.NotEqual(before[5 * width + 2 * Tile], after[5 * width + 2 * Tile]);
        Assert.Equal(before[5 * width + Tile], after[5 * width + Tile]);
        Assert.NotEqual(before[3 * Tile * width + 5], after[3 * Tile * width + 5]);       // the top of the block under the first one
    }

    [Theory]
    [InlineData(TileSheet.B)]
    [InlineData(TileSheet.A1)]
    [InlineData(TileSheet.A4)]
    public void TheNumbersAreWrittenOnTheSheet(TileSheet which)
    {
        var family = SixLabors.Fonts.SystemFonts.Families.FirstOrDefault();
        if (family.Name == null)
            return;   // a machine without a font cannot write the numbers

        var sheet = Sheet(which, which == TileSheet.A1 ? 12 : which == TileSheet.A4 ? 15 : 16);
        var options = new TilesetSheetImageOptions { Grid = false, Background = new Rgba32(0, 0, 0, 255), FontFamily = family };

        using var plain = TilesetSheetImage.Render(sheet, which, new TilesetSheetImageOptions { Grid = false, Labels = false, Background = options.Background });
        using var labelled = TilesetSheetImage.Render(sheet, which, options);

        Assert.NotEqual(Pixels(plain), Pixels(labelled));
    }
}
