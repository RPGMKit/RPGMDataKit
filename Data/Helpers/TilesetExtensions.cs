using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;

namespace RPGMDataKit.Data.Helpers;

/// <summary>
/// The sheets and the flags of a tileset by tile id. The flags are one number for each tile id (<see cref="TileFlags"/>; the terrain tag is the upper 4 bits).
/// A tileset may have fewer flags than there are tile ids (the editor of MV writes one for a new tileset): a tile past the end has no flags. See <c>.claude-docs/maps-and-tiles.md</c>.
/// </summary>
public static class TilesetExtensions
{
    /// <summary>The name of the sheet in the tileset (the file without the extension), or "" for no sheet.</summary>
    public static string SheetName(this ITileSet tileset, TileSheet sheet)
    {
        var index = (int)sheet;

        return index < tileset.TilesetNames.Count ? tileset.TilesetNames[index] ?? "" : "";
    }

    /// <summary>Sets the name of the sheet; the list of names grows to the 9 of a tileset when it is shorter.</summary>
    public static void SetSheetName(this ITileSet tileset, TileSheet sheet, string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        while (tileset.TilesetNames.Count < 9)
            tileset.TilesetNames.Add("");

        tileset.TilesetNames[(int)sheet] = name;
    }

    /// <summary>The name of the sheet that the tile is cut from, or "" for no sheet or a tile that has none (0, an unused id).</summary>
    public static string SheetNameOf(this ITileSet tileset, int tileId) =>
        TileIds.SheetOf(tileId) is { } sheet ? tileset.SheetName(sheet) : "";

    /// <summary>The flags of the tile; none (0) for a tile past the end of the flags.</summary>
    public static int GetFlags(this ITileSet tileset, int tileId) =>
        tileset.Flags.TryGet(tileId, 0, 0, out var flags) ? flags : 0;

    /// <summary>
    /// Sets the flags of the tile (0-32767: the terrain tag takes the bits 12-14, as the editors offer 0-7; a VX Ace file keeps the cells as signed 16-bit numbers).
    /// The table grows to the tile with the flags 0 when it is shorter.
    /// </summary>
    public static void SetFlags(this ITileSet tileset, int tileId, int flags)
    {
        if ((uint)tileId >= TileIds.Max)
            throw new ArgumentOutOfRangeException(nameof(tileId), tileId, $"A tile id is 0-{TileIds.Max - 1}");
        if ((uint)flags > short.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(flags), flags, "The flags of a tile are 0-32767");

        var table = tileset.Flags;
        if (tileId >= table.Count)
        {
            table.Elements.AddRange(new int[tileId + 1 - table.Count]);
            table.Reshape(table.Elements.Count);
        }

        table[tileId] = flags;
    }

    /// <summary>True if the tile has all of the bits (<c>TileFlags.Ladder | TileFlags.Bush</c>).</summary>
    public static bool HasFlags(this ITileSet tileset, int tileId, TileFlags flags) =>
        (tileset.GetFlags(tileId) & (int)flags) == (int)flags;

    /// <summary>The terrain tag of the tile (the upper bits of the flags; the editors offer 0-7).</summary>
    public static int GetTerrainTag(this ITileSet tileset, int tileId) => (tileset.GetFlags(tileId) >> 12) & 0xF;

    /// <summary>Sets the terrain tag (0-7) and keeps the other flags.</summary>
    public static void SetTerrainTag(this ITileSet tileset, int tileId, int tag)
    {
        if ((uint)tag > 7)
            throw new ArgumentOutOfRangeException(nameof(tag), tag, "A terrain tag is 0-7");

        tileset.SetFlags(tileId, (tileset.GetFlags(tileId) & 0x0FFF) | (tag << 12));
    }

    /// <summary>
    /// True if the tile blocks moving in the direction (its own bit; the engines also let a tile with the star flag
    /// <see cref="TileFlags.IsHigherTile"/> pass the decision to the tiles under it). <see cref="Direction.Retain"/> is not a direction.
    /// </summary>
    public static bool BlocksDirection(this ITileSet tileset, int tileId, Direction direction)
    {
        var bit = direction switch
        {
            Direction.Down => (int)TileFlags.PassNoDown,
            Direction.Left => (int)TileFlags.PassNoLeft,
            Direction.Right => (int)TileFlags.PassNoRight,
            Direction.Up => (int)TileFlags.PassNoUp,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Down, Left, Right or Up"),
        };

        return (tileset.GetFlags(tileId) & bit) != 0;
    }
}
