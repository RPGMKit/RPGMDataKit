namespace RPGMDataKit.Data.Structures.Internal;

/// <summary>
/// The flags that the editor gives to the tiles of a new tileset (taken from a tileset made by the editor of VX Ace): every tile
/// can be passed in every direction and has no other flag, the tiles of the autotiles have the flags of their shapes.
/// </summary>
public static class DefaultTilesetFlags
{
    private const int Count = 8192;

    // The low 4 bits of the flags of a tile are the directions in which it cannot be passed (1 = cannot). The same in VX Ace and MV.
    private const int NoDown = 0x1;
    private const int NoLeft = 0x2;
    private const int NoRight = 0x4;
    private const int NoUp = 0x8;

    /// <summary>The flags that the editor sets for the autotiles of walls and ceilings besides the passages (bits 9, 10 and 11).</summary>
    private const int Wall = 0x0E00;

    /// <summary>
    /// The flags of the 48 shapes of one autotile of walls: the edge of a shape that is next to another tile cannot be passed.
    /// The first 16 shapes are the middle of the wall, the others are the sides and the corners.
    /// </summary>
    private static readonly int[] AutotileShapes =
    [
        .. Repeat(Wall, 16),
        .. Repeat(Wall | NoLeft, 4),
        .. Repeat(Wall | NoUp, 4),
        .. Repeat(Wall | NoRight, 4),
        .. Repeat(Wall, 4),
        Wall | NoLeft | NoRight,
        Wall | NoUp,
        Wall | NoLeft | NoUp, Wall | NoLeft | NoUp,
        Wall | NoRight | NoUp, Wall | NoRight | NoUp,
        Wall | NoRight, Wall | NoRight,
        Wall | NoLeft, Wall | NoLeft,
        Wall | NoLeft | NoRight | NoUp,
        Wall | NoLeft | NoUp,
        Wall | NoLeft | NoRight,
        Wall | NoRight | NoUp,
        Wall | NoLeft | NoRight | NoUp,
        Wall | NoDown | NoLeft | NoRight | NoUp,
    ];

    /// <summary>The flags of a new tileset: a table of 8192 numbers.</summary>
    public static Table Create()
    {
        var flags = new Table(Count);
        flags[0] = 0x0010;

        Fill(flags, 1, 1023, 1536);
        Fill(flags, 1536, 128, 1536);

        // the first autotiles (the water and others): 16 kinds of 48 tiles
        var start = 2048;
        foreach (var (value, kinds) in new (int, int)[] { (2063, 1), (2575, 1), (3599, 2), (2063, 1), (3599, 1), (2063, 1), (3599, 1), (2063, 1), (3599, 1), (2063, 1), (3599, 1), (2063, 1), (3599, 1), (2063, 1), (3599, 1) })
        {
            Fill(flags, start, kinds * 48, value);
            start += kinds * 48;
        }

        Fill(flags, 2816, 1536, 1536);
        Fill(flags, 4352, 1536, 3599);

        // the autotiles of the walls and others: three groups of 8 kinds with the shapes and 8 kinds that are passable
        start = 5888;
        for (var group = 0; group < 3; group++)
        {
            for (var kind = 0; kind < 8; kind++)
            {
                for (var shape = 0; shape < 48; shape++)
                    flags[start + shape] = AutotileShapes[shape];

                start += 48;
            }

            Fill(flags, start, 384, 3599);
            start += 384;
        }

        return flags;
    }

    private static void Fill(Table flags, int from, int count, int value)
    {
        for (var i = 0; i < count; i++)
            flags[from + i] = value;
    }

    private static IEnumerable<int> Repeat(int value, int count) => Enumerable.Repeat(value, count);
}
