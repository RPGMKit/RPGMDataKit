using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;

namespace RPGMDataKit.Data.Helpers;

/// <summary>
/// The tiles of a map without the difference of the layout of the makers: VX Ace keeps 4 layers in <c>IMap.Data</c> (3 of tiles; the 4th holds the shadows in its
/// low bits and the regions from the 9th bit), MV keeps 6 (4 of tiles, the shadows, the regions). See <c>.claude-docs/maps-and-tiles.md</c>.
/// The layout is told by the class of the map (the maker's), else by the number of layers in <c>Data</c>.
/// <para>
/// The layer of a tile is the third coordinate of <c>Data</c>: 0 is the lowest. The number of them depends on the maker (<see cref="TileLayerCount"/>), and the same
/// layer is not the same thing in the two: VX Ace has the ground, a second ground layer and one upper layer, MV the ground, a second ground layer and two upper layers.
/// </para>
/// </summary>
public static class MapTileExtensions
{
    /// <summary>The highest region that fits a VX Ace map: the region is stored from the 9th bit of a signed 16-bit cell.</summary>
    public const int MaxRegionVxAce = 127;

    private enum Layout
    {
        VxAce,
        Mv,
    }

    private static Layout LayoutOf(IMap map)
    {
        if (map is Data.VXAce.Classes.Map)
            return Layout.VxAce;
        if (map is Data.MV.Classes.Map)
            return Layout.Mv;

        return map.Data.SizeZ switch
        {
            4 => Layout.VxAce,
            6 => Layout.Mv,
            _ => throw new InvalidOperationException($"A map with {map.Data.SizeZ} layers is neither VX Ace (4) nor MV (6)"),
        };
    }

    /// <summary>The number of layers of tiles: 3 in VX Ace, 4 in MV.</summary>
    public static int TileLayerCount(this IMap map) => LayoutOf(map) == Layout.VxAce ? 3 : 4;

    /// <summary>The tile id on the layer (0 is the lowest) at the square; 0 is nothing.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The layer or the square is outside of the map.</exception>
    public static int GetTile(this IMap map, int x, int y, int layer)
    {
        CheckSquare(map, x, y);
        CheckLayer(map, layer);

        return map.Data[x, y, layer];
    }

    /// <summary>Puts a tile on the layer at the square. The id is 0 (nothing) or a tile id below <see cref="TileIds.Max"/>.</summary>
    public static void SetTile(this IMap map, int x, int y, int layer, int tileId)
    {
        CheckSquare(map, x, y);
        CheckLayer(map, layer);
        if ((uint)tileId >= TileIds.Max)
            throw new ArgumentOutOfRangeException(nameof(tileId), tileId, $"A tile id is 0-{TileIds.Max - 1}");

        map.Data[x, y, layer] = tileId;
    }

    /// <summary>The tiles at the square from the highest layer down, as the engine looks at them (the passage, the terrain tag): 3 ids in VX Ace, 4 in MV.</summary>
    public static IReadOnlyList<int> LayeredTiles(this IMap map, int x, int y)
    {
        CheckSquare(map, x, y);
        var count = map.TileLayerCount();
        var tiles = new int[count];
        for (var i = 0; i < count; i++)
            tiles[i] = map.Data[x, y, count - 1 - i];

        return tiles;
    }

    /// <summary>The shadows of the square, four bits (0-15): bit 0 the top left quarter, 1 the top right, 2 the bottom left, 3 the bottom right.</summary>
    public static int GetShadow(this IMap map, int x, int y)
    {
        CheckSquare(map, x, y);

        return ShadowCell(map, x, y) & (int)ShadowFlag.All;
    }

    /// <summary>Sets the shadows of the square (0-15). The region of the square in a VX Ace map, which shares the cell, is kept.</summary>
    public static void SetShadow(this IMap map, int x, int y, int shadow)
    {
        CheckSquare(map, x, y);
        if ((uint)shadow > (uint)ShadowFlag.All)
            throw new ArgumentOutOfRangeException(nameof(shadow), shadow, "A shadow is four bits, 0-15");

        var z = ShadowLayer(map);
        map.Data[x, y, z] = (map.Data[x, y, z] & ~(int)ShadowFlag.All) | shadow;
    }

    /// <summary>The shadows of the square as the quarters that are dark.</summary>
    public static ShadowFlag GetShadowFlags(this IMap map, int x, int y) => (ShadowFlag)map.GetShadow(x, y);

    /// <summary>Sets the shadows of the square: <c>ShadowFlag.TopLeft | ShadowFlag.BottomRight</c>. The bits that are not a quarter are refused.</summary>
    public static void SetShadow(this IMap map, int x, int y, ShadowFlag shadow) => map.SetShadow(x, y, (int)shadow);

    /// <summary>The region of the square (0 is none).</summary>
    public static int GetRegion(this IMap map, int x, int y)
    {
        CheckSquare(map, x, y);

        return LayoutOf(map) == Layout.VxAce ? map.Data[x, y, 3] >> 8 : map.Data[x, y, 5];
    }

    /// <summary>
    /// Sets the region of the square. A VX Ace map keeps it in the same cell as the shadows (the shadows are kept) and the cell is a signed 16-bit number,
    /// so the region is 0-<see cref="MaxRegionVxAce"/>; an MV map only needs a number that is not negative.
    /// </summary>
    public static void SetRegion(this IMap map, int x, int y, int region)
    {
        CheckSquare(map, x, y);
        if (LayoutOf(map) == Layout.VxAce)
        {
            if ((uint)region > MaxRegionVxAce)
                throw new ArgumentOutOfRangeException(nameof(region), region, $"A region of a VX Ace map is 0-{MaxRegionVxAce}");

            map.Data[x, y, 3] = (map.Data[x, y, 3] & 0xFF) | (region << 8);
        }
        else
        {
            if (region < 0)
                throw new ArgumentOutOfRangeException(nameof(region), region, "A region is not negative");

            map.Data[x, y, 5] = region;
        }
    }

    /// <summary>
    /// Sets the size of a map and makes the tiles empty: the table has the layers of the maker and every cell is 0. (A new map has no events to move.)
    /// </summary>
    internal static void Resize(this IMap map, int width, int height)
    {
        var layers = map.Data.SizeZ;
        map.Width = width;
        map.Height = height;
        map.Data.Elements = new List<int>(new int[width * height * layers]);
        map.Data.Reshape(width, height, layers);
    }

    private static int ShadowLayer(IMap map) => LayoutOf(map) == Layout.VxAce ? 3 : 4;

    private static int ShadowCell(IMap map, int x, int y) => map.Data[x, y, ShadowLayer(map)];

    private static void CheckSquare(IMap map, int x, int y)
    {
        if ((uint)x >= (uint)map.Width)
            throw new ArgumentOutOfRangeException(nameof(x), x, $"The map is {map.Width} squares wide");
        if ((uint)y >= (uint)map.Height)
            throw new ArgumentOutOfRangeException(nameof(y), y, $"The map is {map.Height} squares high");
    }

    private static void CheckLayer(IMap map, int layer)
    {
        var count = map.TileLayerCount();
        if ((uint)layer >= (uint)count)
            throw new ArgumentOutOfRangeException(nameof(layer), layer, $"A map of this maker has {count} layers of tiles (0-{count - 1})");
    }
}
