using System.Security.Cryptography;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Imaging;
using RPGMDataKit.Storages;
using RPGMDataKit.Storages.Abstraction;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Xunit;

namespace RPGMDataKit.Tests;

/// <summary>
/// The picture of a map (<see cref="MapRenderer"/>). The drawing is compared with the code of the engine of MV: <see cref="TileScenario"/> is a made-up map and tileset that has every kind of tile,
/// and <c>EngineHashOfTheScenario</c> is the hash of the picture that <c>Tilemap</c> of <c>rpg_core.js</c> draws of it (run by a script on the dumped scenario, see the variable named in
/// <see cref="TileScenario"/>), so the tests need no engine to be run.
/// </summary>
public sealed class MapRendererTests : IDisposable
{
    // the hash of the picture the engine draws of TileScenario.Create(1), the frame 0
    private const string EngineHashOfTheScenario = "6a8349895263d226c9f29672a8981a0d270144db5064c3e19f6ba9977dc559f8";

    private readonly string _folder = Path.Combine(Path.GetTempPath(), "RPGMDataKit.Tests." + Guid.NewGuid().ToString("N"));

    public MapRendererTests() => Directory.CreateDirectory(_folder);

    public void Dispose() => Directory.Delete(_folder, true);

    public static IEnumerable<object[]> Versions() => Version.All.Select(v => new object[] { v });

    private static Image<Rgba32> Render(TileScenario scenario, Version version, MapImageOptions? options = null) =>
        MapRenderer.Render(scenario.ToMap(version), scenario.ToTileset(version), scenario.ToSheets(), options);

    private static Rgba32[] Pixels(Image<Rgba32> image)
    {
        var pixels = new Rgba32[image.Width * image.Height];
        image.CopyPixelDataTo(pixels);

        return pixels;
    }

    private static string Hash(Image<Rgba32> image)
    {
        var pixels = Pixels(image);
        var bytes = new byte[pixels.Length * 4];
        for (var i = 0; i < pixels.Length; i++)
        {
            bytes[i * 4] = pixels[i].R;
            bytes[i * 4 + 1] = pixels[i].G;
            bytes[i * 4 + 2] = pixels[i].B;
            bytes[i * 4 + 3] = pixels[i].A;
        }

        return Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    }

    [Fact]
    public void TheMapIsDrawnAsTheEngineDrawsIt()
    {
        var scenario = TileScenario.Create(1);
        if (Environment.GetEnvironmentVariable("RPGMDATAKIT_TILE_SCENARIO_DUMP") is { Length: > 0 } dump)
        {
            scenario.Dump(dump);
            File.WriteAllText(Path.Combine(dump, "csharp.sha256"), Hash(Render(scenario, Version.Mv)));
        }

        using var image = Render(scenario, Version.Mv);

        Assert.Equal((TileScenario.TileSize * scenario.Width, TileScenario.TileSize * scenario.Height), (image.Width, image.Height));
        Assert.Equal(EngineHashOfTheScenario, Hash(image));
    }

    [Fact]
    public void AMapOfVxAceIsDrawnLikeTheSameMapOfMv()
    {
        // VX Ace has three layers of tiles; with the fourth empty the two are the same map
        var scenario = TileScenario.Create(1);
        var emptyTop = TileScenario.WithoutTheTopLayer(scenario);

        using var mv = Render(emptyTop, Version.Mv);
        using var vx = Render(emptyTop, Version.VXAce);

        Assert.Equal(Hash(mv), Hash(vx));
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void APartIsTheCutOfTheWholePicture(Version v)
    {
        var scenario = TileScenario.Create(2);
        var size = TileScenario.TileSize;
        using var whole = Render(scenario, v);

        foreach (var region in new[] { new MapRegion(0, 0, 1, 1), new MapRegion(3, 2, 7, 5), new MapRegion(0, 5, 20, 11), new MapRegion(19, 15, 1, 1), new MapRegion(5, 0, 6, 16) })
        {
            using var part = Render(scenario, v, new MapImageOptions { Region = region });

            Assert.Equal((region.Width * size, region.Height * size), (part.Width, part.Height));
            var expected = whole.Clone(c => c.Crop(new Rectangle(region.X * size, region.Y * size, region.Width * size, region.Height * size)));
            Assert.Equal(Pixels(expected), Pixels(part));
        }
    }

    [Theory]
    [MemberData(nameof(Versions))]
    public void APartOutsideOfTheMapIsRefused(Version v)
    {
        var scenario = TileScenario.Create(2);

        foreach (var region in new[] { new MapRegion(-1, 0, 2, 2), new MapRegion(0, -1, 2, 2), new MapRegion(19, 0, 2, 1), new MapRegion(0, 15, 1, 2), new MapRegion(0, 0, 0, 1), new MapRegion(0, 0, 1, 0), new MapRegion(0, 0, 21, 16) })
            Assert.Throws<ArgumentOutOfRangeException>(() => Render(scenario, v, new MapImageOptions { Region = region }));
    }

    [Fact]
    public void TheLowerAndTheUpperLayersMakeTheWholePicture()
    {
        var scenario = TileScenario.Create(3);
        using var both = Render(scenario, Version.Mv);
        using var lower = Render(scenario, Version.Mv, new MapImageOptions { Layers = MapImageLayers.Lower });
        using var upper = Render(scenario, Version.Mv, new MapImageOptions { Layers = MapImageLayers.Upper });

        // the upper layer over the lower one
        var composed = Pixels(lower);
        var over = Pixels(upper);
        for (var i = 0; i < composed.Length; i++)
            PixelBlend(ref composed[i], over[i]);

        Assert.Equal(Pixels(both), composed);
        Assert.NotEqual(Pixels(both), Pixels(lower));   // the scenario has tiles with the star flag
        Assert.Contains(Pixels(upper), p => p.A == 0);   // and the upper layer has only some of the squares
    }

    private static void PixelBlend(ref Rgba32 under, Rgba32 over)
    {
        // the pixels of the scenario are clear or whole: the upper one covers the lower one where it is whole
        if (over.A != 0)
            under = over;
    }

    [Fact]
    public void TheShadowsCanBeLeftOut()
    {
        var scenario = TileScenario.Create(4);
        using var with = Render(scenario, Version.Mv);
        using var without = Render(scenario, Version.Mv, new MapImageOptions { Shadows = false });

        Assert.NotEqual(Pixels(with), Pixels(without));
        // a shadow darkens, never lightens
        var a = Pixels(with);
        var b = Pixels(without);
        Assert.All(Enumerable.Range(0, a.Length), i => Assert.True(a[i].R <= b[i].R || a[i].A != b[i].A));
    }

    private static readonly Rgba32 White = new(255, 255, 255, 255);

    private static readonly Rgba32 Black = new(0, 0, 0, 255);

    // a light line at the left and the top of a square and at the far edges of the picture, a dark one of the same width next to it; the light one is on top where they cross
    private static int LineKind(int x, int y, int width, int height, int cell, int lineWidth)
    {
        static int Axis(int offset, int fromEnd, int lineWidth) =>
            offset < lineWidth || fromEnd < lineWidth ? 2 : offset < 2 * lineWidth || fromEnd < 2 * lineWidth ? 1 : 0;

        return Math.Max(Axis(x % cell, width - 1 - x, lineWidth), Axis(y % cell, height - 1 - y, lineWidth));
    }

    private static Rgba32 AfterTheGrid(Rgba32 before, int kind, Rgba32 light, Rgba32 dark) => kind switch
    {
        2 => light,
        1 => dark,
        _ => before,
    };

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void TheGridIsALightLineAndADarkOneBetweenTheSquaresAndTheRestOfThePictureStays(int lineWidth)
    {
        var scenario = TileScenario.Create(1);
        using var plain = Render(scenario, Version.Mv);
        using var gridded = Render(scenario, Version.Mv, new MapImageOptions { Grid = new MapGridOptions { Color = White, ShadowColor = Black, LineWidth = lineWidth } });

        Assert.Equal((plain.Width, plain.Height), (gridded.Width, gridded.Height));
        var before = Pixels(plain);
        var after = Pixels(gridded);
        for (var i = 0; i < after.Length; i++)
        {
            var (x, y) = (i % plain.Width, i / plain.Width);
            var expected = AfterTheGrid(before[i], LineKind(x, y, plain.Width, plain.Height, TileScenario.TileSize, lineWidth), White, Black);
            Assert.True(expected == after[i], $"pixel {x},{y}: {after[i]} instead of {expected}");
        }
    }

    [Fact]
    public void ATransparentShadowColourMakesALineOfOneColour()
    {
        var scenario = TileScenario.Create(1);
        using var plain = Render(scenario, Version.Mv);
        using var gridded = Render(scenario, Version.Mv, new MapImageOptions { Grid = new MapGridOptions { Color = White, ShadowColor = default } });

        var before = Pixels(plain);
        var after = Pixels(gridded);
        for (var i = 0; i < after.Length; i++)
        {
            var (x, y) = (i % plain.Width, i / plain.Width);
            var kind = LineKind(x, y, plain.Width, plain.Height, TileScenario.TileSize, 1);
            Assert.Equal(kind == 2 ? White : before[i], after[i]);
        }
    }

    [Fact]
    public void AGridCanBeSeenOnALightAndOnADarkMap()
    {
        // the default lines: whatever the colour under them, one of the two lines is not the colour of the map
        var scenario = TileScenario.Create(1);
        foreach (var background in new[] { new Rgba32(255, 255, 255, 255), new Rgba32(0, 0, 0, 255) })
        {
            var map = scenario.ToMap(Version.Mv);
            var tileset = scenario.ToTileset(Version.Mv);
            var empty = new InMemoryTilesetSheets(new Dictionary<TileSheet, TilesetSheet>());
            using var plain = MapRenderer.Render(map, tileset, empty, new MapImageOptions { TileSize = TileScenario.TileSize, Background = background });
            using var gridded = MapRenderer.Render(map, tileset, empty, new MapImageOptions { TileSize = TileScenario.TileSize, Background = background, Grid = new MapGridOptions() });

            var before = Pixels(plain);
            var after = Pixels(gridded);
            var lightLine = after[0];
            var darkLine = after[plain.Width + 1];   // the first dark pixel: inside of the first square, one line in from the corner
            Assert.True(before[0] != lightLine || before[0] != darkLine, $"the lines are not seen on {background}");
            Assert.NotEqual(lightLine, darkLine);
        }
    }

    [Fact]
    public void TheLinesOfTheGridAreLaidOverTheTilesByTheirAlpha()
    {
        var scenario = TileScenario.Create(1);
        using var plain = Render(scenario, Version.Mv);
        using var gridded = Render(scenario, Version.Mv, new MapImageOptions { Grid = new MapGridOptions { Color = new Rgba32(0, 0, 0, 128) } });

        // the first pixel of the scenario is a whole one: black with the alpha of a half over it keeps 127/255 of each channel
        var before = Pixels(plain)[0];
        var after = Pixels(gridded)[0];

        Assert.Equal(255, before.A);
        Assert.Equal(255, after.A);
        Assert.InRange(after.R, before.R * 127 / 255 - 1, before.R * 127 / 255 + 1);
        Assert.InRange(after.G, before.G * 127 / 255 - 1, before.G * 127 / 255 + 1);
        Assert.InRange(after.B, before.B * 127 / 255 - 1, before.B * 127 / 255 + 1);
    }

    [Fact]
    public void TheGridOfAPartStartsAtTheCornerOfThePart()
    {
        var scenario = TileScenario.Create(1);
        var region = new MapRegion(2, 1, 3, 2);
        using var gridded = Render(scenario, Version.Mv, new MapImageOptions { Region = region, Grid = new MapGridOptions { Color = White, ShadowColor = Black } });
        using var plain = Render(scenario, Version.Mv, new MapImageOptions { Region = region });

        var after = Pixels(gridded);
        var before = Pixels(plain);
        for (var i = 0; i < after.Length; i++)
        {
            var (x, y) = (i % plain.Width, i / plain.Width);
            Assert.Equal(AfterTheGrid(before[i], LineKind(x, y, plain.Width, plain.Height, TileScenario.TileSize, 1), White, Black), after[i]);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(TileScenario.TileSize / 4 + 1)]
    public void ALineThatIsNotFromOnePixelToAQuarterOfATileIsRefused(int lineWidth)
    {
        var scenario = TileScenario.Create(1);

        Assert.Throws<ArgumentOutOfRangeException>(() => Render(scenario, Version.Mv, new MapImageOptions { Grid = new MapGridOptions { LineWidth = lineWidth } }));
    }

    // the texts need a font of the system; a machine that has none cannot run these
    private static MapGridOptions? Labelled(Func<int, int, string?>? label = null, GridLabels labels = GridLabels.None, int layer = 0)
    {
        var family = SixLabors.Fonts.SystemFonts.Families.FirstOrDefault();
        if (family.Name == null)
            return null;

        return new MapGridOptions { Color = White, FontFamily = family, Label = label, Labels = labels, TileLayer = layer };
    }

    [Fact]
    public void ATextIsWrittenInItsSquareOnly()
    {
        var lines = new MapGridOptions { Color = White };
        var labelled = Labelled((x, y) => x == 1 && y == 2 ? "7" : null);
        if (labelled == null)
            return;

        var scenario = TileScenario.Create(1);
        using var withLines = Render(scenario, Version.Mv, new MapImageOptions { Grid = lines });
        using var withText = Render(scenario, Version.Mv, new MapImageOptions { Grid = labelled });

        var size = TileScenario.TileSize;
        var before = Pixels(withLines);
        var after = Pixels(withText);
        var changed = 0;
        for (var i = 0; i < after.Length; i++)
        {
            if (before[i] == after[i])
                continue;

            changed++;
            var (x, y) = (i % withLines.Width, i / withLines.Width);
            Assert.True(x / size == 1 && y / size == 2, $"pixel {x},{y} is not in the square 1,2");
        }

        Assert.True(changed > 0, "nothing was written");
    }

    [Fact]
    public void ANoTextOrAnEmptyOneWritesNothing()
    {
        var labelled = Labelled((x, y) => y == 0 ? "" : null);
        if (labelled == null)
            return;

        var scenario = TileScenario.Create(1);
        using var withLines = Render(scenario, Version.Mv, new MapImageOptions { Grid = new MapGridOptions { Color = White } });
        using var withText = Render(scenario, Version.Mv, new MapImageOptions { Grid = labelled });

        Assert.Equal(Pixels(withLines), Pixels(withText));
    }

    [Fact]
    public void TheTextOfASquareIsAskedForWithThePlaceInTheMapAlsoWhenAPartIsDrawn()
    {
        var asked = new List<(int, int)>();
        var labelled = Labelled((x, y) =>
        {
            asked.Add((x, y));

            return null;
        });
        if (labelled == null)
            return;

        using var image = Render(TileScenario.Create(1), Version.Mv, new MapImageOptions { Region = new MapRegion(2, 1, 3, 2), Grid = labelled });

        Assert.Equal([(2, 1), (3, 1), (4, 1), (2, 2), (3, 2), (4, 2)], asked);
    }

    [Theory]
    [InlineData(GridLabels.Coordinates, 0, true)]
    [InlineData(GridLabels.TileId, 0, true)]
    [InlineData(GridLabels.TileId, 9, false)]   // a layer the map does not have: no tile, nothing written
    [InlineData(GridLabels.None, 0, false)]
    public void TheReadyTextsAreWrittenWhereThereIsSomethingToSay(GridLabels labels, int layer, bool writes)
    {
        var labelled = Labelled(null, labels, layer);
        if (labelled == null)
            return;

        var scenario = TileScenario.Create(1);
        using var withLines = Render(scenario, Version.Mv, new MapImageOptions { Grid = new MapGridOptions { Color = White } });
        using var withText = Render(scenario, Version.Mv, new MapImageOptions { Grid = labelled });

        Assert.Equal(writes, !Pixels(withLines).SequenceEqual(Pixels(withText)));
    }

    [Fact]
    public void TheTextCanStandInTheMiddleOfTheSquare()
    {
        var family = SixLabors.Fonts.SystemFonts.Families.FirstOrDefault();
        if (family.Name == null)
            return;

        var scenario = TileScenario.Create(1);
        Func<int, int, string?> one = (x, y) => x == 0 && y == 0 ? "5" : null;
        using var corner = Render(scenario, Version.Mv, new MapImageOptions { Grid = new MapGridOptions { FontFamily = family, Label = one } });
        using var middle = Render(scenario, Version.Mv, new MapImageOptions { Grid = new MapGridOptions { FontFamily = family, Label = one, LabelPosition = GridLabelPosition.Center } });

        Assert.NotEqual(Pixels(corner), Pixels(middle));
    }

    [Fact]
    public void TheBackgroundShowsWhereThereIsNoTile()
    {
        var empty = TileScenario.Empty(4, 3);
        using var clear = Render(empty, Version.Mv);
        using var red = Render(empty, Version.Mv, new MapImageOptions { Background = new Rgba32(255, 0, 0, 255) });

        Assert.All(Pixels(clear), p => Assert.Equal(0, p.A));
        Assert.All(Pixels(red), p => Assert.Equal(new Rgba32(255, 0, 0, 255), p));
    }

    [Fact]
    public void TheWaterMovesWithTheFrameAndTheRestOfTheMapDoesNot()
    {
        var scenario = TileScenario.OneSquare(TileIds.MakeAutotile(0, 0));
        var ground = TileScenario.OneSquare(TileIds.MakeAutotile(16, 3));

        var frames = Enumerable.Range(0, 4).Select(f => Hash(Render(scenario, Version.Mv, new MapImageOptions { AnimationFrame = f }))).ToList();
        Assert.Equal(frames[0], Hash(Render(scenario, Version.Mv)));
        Assert.NotEqual(frames[0], frames[1]);
        Assert.NotEqual(frames[1], frames[2]);
        Assert.Equal(frames[1], frames[3]);   // the water goes 0, 1, 2, 1
        Assert.Equal(frames[0], Hash(Render(scenario, Version.Mv, new MapImageOptions { AnimationFrame = 4 })));

        var groundFrames = Enumerable.Range(0, 4).Select(f => Hash(Render(ground, Version.Mv, new MapImageOptions { AnimationFrame = f }))).Distinct().ToList();
        Assert.Single(groundFrames);
    }

    [Fact]
    public void ASheetThatIsMissingDrawsNothingAndTheSizeComesFromTheOthers()
    {
        var scenario = TileScenario.OneSquare(TileIds.MakeAutotile(16, 0));
        var map = scenario.ToMap(Version.Mv);
        var without = scenario.Sheets.Where(p => p.Key != TileSheet.A2).ToDictionary(p => p.Key, p => p.Value);

        using var image = MapRenderer.Render(map, scenario.ToTileset(Version.Mv), new InMemoryTilesetSheets(without));

        Assert.Equal((TileScenario.TileSize, TileScenario.TileSize), (image.Width, image.Height));
        Assert.All(Pixels(image), p => Assert.Equal(0, p.A));
    }

    [Fact]
    public void WithNoSheetTheSizeMustBeGivenAndSheetsThatDisagreeAreRefused()
    {
        var scenario = TileScenario.OneSquare(1);
        var map = scenario.ToMap(Version.Mv);
        var tileset = scenario.ToTileset(Version.Mv);

        Assert.Throws<InvalidOperationException>(() => MapRenderer.Render(map, tileset, new InMemoryTilesetSheets(new Dictionary<TileSheet, TilesetSheet>())));

        using var image = MapRenderer.Render(map, tileset, new InMemoryTilesetSheets(new Dictionary<TileSheet, TilesetSheet>()), new MapImageOptions { TileSize = 16 });
        Assert.Equal((16, 16), (image.Width, image.Height));

        var odd = new Dictionary<TileSheet, TilesetSheet>
        {
            [TileSheet.B] = new(16 * 8, 16 * 8, new Rgba32[16 * 8 * 16 * 8]),
            [TileSheet.C] = new(16 * 12, 16 * 12, new Rgba32[16 * 12 * 16 * 12]),
        };
        Assert.Throws<InvalidOperationException>(() => MapRenderer.Render(map, tileset, new InMemoryTilesetSheets(odd)));
        Assert.Throws<ArgumentOutOfRangeException>(() => MapRenderer.Render(map, tileset, scenario.ToSheets(), new MapImageOptions { TileSize = 10 }));
    }

    // ---- through the storage and the files of the project ----

    [Theory]
    [MemberData(nameof(Versions))]
    public void ThePictureOfAMapIsDrawnFromTheSheetsInTheProject(Version v)
    {
        var project = Path.Combine(_folder, "project");
        foreach (var file in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Projects", v.Name), "*", SearchOption.AllDirectories))
        {
            var target = Path.Combine(project, Path.GetRelativePath(Path.Combine(AppContext.BaseDirectory, "Projects", v.Name), file));
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(file, target);
        }

        if (v == Version.VXAce)
            v.Serializer.Save(Path.Combine(project, "Data", "Scripts.rvdata2"), new List<RPGMDataKit.Data.VXAce.Classes.Script>());

        Storage storage = v == Version.VXAce ? new VXAceStorage(project, "Data") : new MVStorage(project, "data");
        var scenario = TileScenario.Create(6, 17, 13);

        // the map 1 gets the tiles of the scenario, the tileset 1 its names, and the sheets are files named by the tileset in the folder of the maker (one with the case changed)
        var map = storage.Maps.Get(1)!;
        var made = scenario.ToMap(v);
        map.Data.Elements = made.Data.Elements;
        map.Data.Reshape(made.Data.SizeX, made.Data.SizeY, made.Data.SizeZ);
        var tileset = storage.Tilesets.Get(map.TilesetId)!;
        for (var id = 0; id < scenario.Flags.Length; id++)
            tileset.SetFlags(id, scenario.Flags[id]);
        foreach (var (sheet, pixels) in scenario.Sheets)
        {
            tileset.SetSheetName(sheet, "Sheet_" + sheet);
            var path = storage.GetResourcePath(ResourceType.Tileset, sheet == TileSheet.A2 ? "SHEET_a2" : "Sheet_" + sheet, ".png");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            using var image = Image.LoadPixelData<Rgba32>(pixels.Pixels, pixels.Width, pixels.Height);
            image.SaveAsPng(path);
        }

        using var picture = storage.RenderMap(1);
        using var expected = MapRenderer.Render(map, tileset, scenario.ToSheets());

        Assert.Equal(Pixels(expected), Pixels(picture));
        using var part = storage.RenderMap(1, new MapImageOptions { Region = new MapRegion(2, 1, 5, 4) });
        Assert.Equal((5 * TileScenario.TileSize, 4 * TileScenario.TileSize), (part.Width, part.Height));
        Assert.Throws<ArgumentException>(() => storage.RenderMap(99));
    }
}
