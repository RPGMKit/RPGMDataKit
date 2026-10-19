using System.Text.Json;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Imaging;
using SixLabors.ImageSharp.PixelFormats;

namespace RPGMDataKit.Tests;

/// <summary>
/// A map, a tileset and the pictures of its sheets made from nothing by a fixed rule, so that the same ones come out every time: the sheets are pixels that tell their place, the tiles are
/// chosen at random from every kind (normal tiles, the autotiles of each sheet, ids that are not used), with random shadows and random star and counter flags. It is small (tiles of 8 pixels)
/// and has everything the drawing of a map does, so the drawing can be compared with the code of the engine.
/// <para>
/// To compare it with the engine set the variable <c>RPGMDATAKIT_TILE_SCENARIO_DUMP</c> to a folder before the tests run: <see cref="Dump"/> writes the scenario there for a script that runs the code of
/// <c>rpg_core.js</c>; the hash of what the script draws is in <see cref="MapRendererTests"/>.
/// </para>
/// </summary>
internal sealed class TileScenario
{
    public const int TileSize = 8;

    public int Width { get; private init; }

    public int Height { get; private init; }

    /// <summary>The four layers of tiles, each Width x Height, row by row.</summary>
    public int[][] Layers { get; private init; } = [];

    /// <summary>The shadows (0-15) of each square.</summary>
    public int[] Shadows { get; private init; } = [];

    /// <summary>The flags of the 8192 tiles.</summary>
    public int[] Flags { get; private init; } = [];

    public Dictionary<TileSheet, TilesetSheet> Sheets { get; private init; } = new();

    public static TileScenario Create(int seed, int width = 20, int height = 16)
    {
        var random = new Xorshift((uint)seed);
        var layers = Enumerable.Range(0, 4).Select(_ => new int[width * height]).ToArray();
        var shadows = new int[width * height];
        for (var i = 0; i < width * height; i++)
        {
            layers[0][i] = Ground(random);
            layers[1][i] = Decoration(random);
            layers[2][i] = Object(random);
            layers[3][i] = Top(random);
            shadows[i] = random.Next(10) < 3 ? random.Next(16) : 0;
        }

        var flags = new int[TileIds.Max];
        for (var id = 0; id < flags.Length; id++)
        {
            var h = Hash((uint)id, 7, 11);
            flags[id] = (h % 4 == 0 ? 0x10 : 0) | (h / 7 % 3 == 0 ? 0x80 : 0);
        }

        var sheets = new Dictionary<TileSheet, TilesetSheet>();
        foreach (var (sheet, columns, rows) in new[]
                 {
                     (TileSheet.A1, 16, 12), (TileSheet.A2, 16, 12), (TileSheet.A3, 16, 8), (TileSheet.A4, 16, 15), (TileSheet.A5, 8, 16),
                     (TileSheet.B, 16, 16), (TileSheet.C, 16, 16), (TileSheet.D, 16, 16), (TileSheet.E, 16, 16),
                 })
        {
            sheets[sheet] = Sheet(sheet, columns * TileSize, rows * TileSize);
        }

        return new TileScenario { Width = width, Height = height, Layers = layers, Shadows = shadows, Flags = flags, Sheets = sheets };
    }

    /// <summary>The same map without the tiles of the 4th layer (the layout of VX Ace has three layers of tiles).</summary>
    public static TileScenario WithoutTheTopLayer(TileScenario scenario) => new()
    {
        Width = scenario.Width,
        Height = scenario.Height,
        Layers = [scenario.Layers[0], scenario.Layers[1], scenario.Layers[2], new int[scenario.Width * scenario.Height]],
        Shadows = scenario.Shadows,
        Flags = scenario.Flags,
        Sheets = scenario.Sheets,
    };

    /// <summary>A map of no tiles and no shadows, with the usual sheets.</summary>
    public static TileScenario Empty(int width, int height) => new()
    {
        Width = width,
        Height = height,
        Layers = Enumerable.Range(0, 4).Select(_ => new int[width * height]).ToArray(),
        Shadows = new int[width * height],
        Flags = new int[TileIds.Max],
        Sheets = Create(1).Sheets,
    };

    /// <summary>A map of one square with one tile on the lowest layer, no flags, with the usual sheets.</summary>
    public static TileScenario OneSquare(int tileId)
    {
        var scenario = Empty(1, 1);
        scenario.Layers[0][0] = tileId;

        return scenario;
    }

    // a pixel of a sheet tells where it is: its colour is a hash of the sheet and the place, and one in four is clear
    private static TilesetSheet Sheet(TileSheet sheet, int width, int height)
    {
        var pixels = new Rgba32[width * height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var h = Hash((uint)sheet, (uint)x, (uint)y);
                pixels[y * width + x] = new Rgba32((byte)h, (byte)(h >> 8), (byte)(h >> 16), h >> 24 % 4 == 0 ? (byte)0 : (byte)255);
            }
        }

        return new TilesetSheet(width, height, pixels);
    }

    private static uint Hash(uint a, uint b, uint c)
    {
        var h = a * 73856093u ^ b * 19349663u ^ c * 83492791u;
        h ^= h >> 15;
        h *= 2246822519u;
        h ^= h >> 13;
        h *= 3266489917u;
        h ^= h >> 16;

        return h;
    }

    private static int Autotile(Xorshift random, int firstKind, int kinds) =>
        TileIds.MakeAutotile(firstKind + random.Next(kinds), random.Next(TileIds.ShapesPerKind));

    private static int Ground(Xorshift random) => random.Next(100) switch
    {
        < 55 => Autotile(random, 16, 32),
        < 70 => TileIds.A5 + random.Next(128),
        < 80 => Autotile(random, 0, 16),
        < 88 => Autotile(random, 80, 48),
        < 95 => Autotile(random, 48, 32),
        _ => 0,
    };

    private static int Decoration(Xorshift random) => random.Next(100) switch
    {
        < 40 => 0,
        < 70 => Autotile(random, 16, 32),
        < 80 => Autotile(random, 0, 16),
        _ => 1 + random.Next(255),
    };

    private static int Object(Xorshift random) => random.Next(100) switch
    {
        < 50 => 0,
        < 90 => 1 + random.Next(1023),
        < 95 => Autotile(random, 80, 48),
        _ => Autotile(random, 48, 32),
    };

    private static int Top(Xorshift random) => random.Next(100) switch
    {
        < 60 => 0,
        < 90 => 1 + random.Next(1023),
        < 95 => 1024 + random.Next(600),   // the ids that are not used, and the end of A5
        _ => Autotile(random, 0, 128),
    };

    /// <summary>The map in the layout of the maker. The layout of VX Ace has no 4th layer of tiles, so its map has the first three.</summary>
    public IMap ToMap(Version version)
    {
        var map = version.New<IMap>();
        var tileLayers = version == Version.VXAce ? 3 : 4;
        var layers = version == Version.VXAce ? 4 : 6;
        var cells = new List<int>(Width * Height * layers);
        for (var z = 0; z < tileLayers; z++)
            cells.AddRange(Layers[z]);

        if (version == Version.VXAce)
            cells.AddRange(Shadows);   // the 4th layer: the shadows in the low bits, no region
        else
        {
            cells.AddRange(Shadows);
            cells.AddRange(new int[Width * Height]);   // the regions
        }

        map.Width = Width;
        map.Height = Height;
        map.Data.Elements = cells;
        map.Data.Reshape(Width, Height, layers);

        return map;
    }

    public ITileSet ToTileset(Version version)
    {
        var tileset = version.New<ITileSet>();
        for (var id = 0; id < Flags.Length; id++)
            tileset.SetFlags(id, Flags[id]);

        return tileset;
    }

    public ITilesetSheets ToSheets() => new InMemoryTilesetSheets(Sheets);

    /// <summary>Writes the scenario for the script that draws it with the code of the engine.</summary>
    public void Dump(string folder)
    {
        Directory.CreateDirectory(folder);
        var sheets = Sheets.OrderBy(p => p.Key).Select(p => new { index = (int)p.Key, width = p.Value.Width, height = p.Value.Height }).ToArray();
        File.WriteAllText(Path.Combine(folder, "scenario.json"), JsonSerializer.Serialize(new
        {
            tileSize = TileSize,
            width = Width,
            height = Height,
            layers = Layers,
            shadows = Shadows,
            flags = Flags,
            sheets,
        }));

        foreach (var (sheet, pixels) in Sheets)
        {
            var bytes = new byte[pixels.Pixels.Length * 4];
            for (var i = 0; i < pixels.Pixels.Length; i++)
            {
                bytes[i * 4] = pixels.Pixels[i].R;
                bytes[i * 4 + 1] = pixels.Pixels[i].G;
                bytes[i * 4 + 2] = pixels.Pixels[i].B;
                bytes[i * 4 + 3] = pixels.Pixels[i].A;
            }

            File.WriteAllBytes(Path.Combine(folder, $"sheet{(int)sheet}.rgba"), bytes);
        }
    }

    private sealed class Xorshift(uint seed)
    {
        private uint _state = seed == 0 ? 2463534242u : seed;

        public int Next(int below)
        {
            _state ^= _state << 13;
            _state ^= _state >> 17;
            _state ^= _state << 5;

            return (int)(_state % (uint)below);
        }
    }
}
