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
