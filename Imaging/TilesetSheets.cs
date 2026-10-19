using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Helpers;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Storages.Abstraction;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RPGMDataKit.Imaging;

/// <summary>The pixels of one sheet of a tileset (A1 ... E), row by row.</summary>
public sealed class TilesetSheet
{
    public TilesetSheet(int width, int height, Rgba32[] pixels)
    {
        if (width < 1 || height < 1)
            throw new ArgumentOutOfRangeException(nameof(width), "A sheet has at least one pixel");
        if (pixels.Length != width * height)
            throw new ArgumentException($"{width} x {height} pixels are {width * height}, not {pixels.Length}", nameof(pixels));

        Width = width;
        Height = height;
        Pixels = pixels;
    }

    public int Width { get; }

    public int Height { get; }

    public Rgba32[] Pixels { get; }

    public static TilesetSheet FromImage(Image<Rgba32> image)
    {
        var pixels = new Rgba32[image.Width * image.Height];
        image.CopyPixelDataTo(pixels);

        return new TilesetSheet(image.Width, image.Height, pixels);
    }

    public static TilesetSheet Load(string path)
    {
        using var image = Image.Load<Rgba32>(path);

        return FromImage(image);
    }

    /// <summary>
    /// The size of a tile of this sheet in pixels: the sheets of B-E, A1-A4 are 16 tiles wide, the sheet of A5 is 8.
    /// </summary>
    public int TileSize(TileSheet sheet) => Width / (sheet == TileSheet.A5 ? 8 : 16);
}

/// <summary>Where the sheets of a tileset come from.</summary>
public interface ITilesetSheets
{
    /// <summary>The sheet, or null when the tileset has none there (no name, or no file).</summary>
    TilesetSheet? Get(TileSheet sheet);
}

/// <summary>Sheets that are already in memory (a test, a generated picture).</summary>
public sealed class InMemoryTilesetSheets(IReadOnlyDictionary<TileSheet, TilesetSheet> sheets) : ITilesetSheets
{
    public TilesetSheet? Get(TileSheet sheet) => sheets.GetValueOrDefault(sheet);
}

/// <summary>
/// The sheets of a tileset in the project: the file of each sheet is found by the storage (<see cref="Storage.FindResourceFile"/> for <see cref="ResourceType.Tileset"/>), which knows the folder of the
/// maker (<c>Graphics/Tilesets</c> of VX Ace, <c>img/tilesets</c> of MV), the extensions it reads and that the case of a name does not matter. The pictures must be files in the folder, not packed
/// in an archive. A sheet is read when it is asked for first, and kept.
/// </summary>
public sealed class StorageTilesetSheets(Storage storage, ITileSet tileset) : ITilesetSheets
{
    private readonly Dictionary<TileSheet, TilesetSheet?> _loaded = new();

    public TilesetSheet? Get(TileSheet sheet)
    {
        if (_loaded.TryGetValue(sheet, out var kept))
            return kept;

        var name = tileset.SheetName(sheet);
        var path = string.IsNullOrWhiteSpace(name) ? null : storage.FindResourceFile(ResourceType.Tileset, name);

        return _loaded[sheet] = path == null ? null : TilesetSheet.Load(path);
    }
}
