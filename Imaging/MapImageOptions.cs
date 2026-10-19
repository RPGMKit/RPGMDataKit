using SixLabors.ImageSharp.PixelFormats;

namespace RPGMDataKit.Imaging;

/// <summary>A rectangle of squares of a map: the square at the top left corner and the width and height in squares.</summary>
public readonly record struct MapRegion(int X, int Y, int Width, int Height);

/// <summary>
/// Which of the two layers of the map to draw, as the engine keeps them: the <b>lower</b> one is under the characters and the events (the ground, the shadows, most objects), the <b>upper</b> one
/// is over them (the tiles with the star flag: tops of trees and roofs). Drawn one after the other they are the whole map; the events and characters of a picture go between them.
/// </summary>
[Flags]
public enum MapImageLayers
{
    Lower = 1,
    Upper = 2,
    Both = Lower | Upper,
}

public sealed class MapImageOptions
{
    /// <summary>The part of the map to draw, in squares; all of the map when it is not given. It must be inside of the map. The picture is as big as the part.</summary>
    public MapRegion? Region { get; init; }

    /// <summary>The layers to draw. Both by default.</summary>
    public MapImageLayers Layers { get; init; } = MapImageLayers.Both;

    /// <summary>Draw the shadows (the dark quarters of the squares).</summary>
    public bool Shadows { get; init; } = true;

    /// <summary>
    /// The frame of the animation of the water and waterfalls of A1 (the engine changes it every few frames): the water goes 0, 1, 2, 1 and the waterfalls 0, 1, 2.
    /// The first frame by default.
    /// </summary>
    public int AnimationFrame { get; init; }

    /// <summary>The colour under the tiles; transparent by default.</summary>
    public Rgba32 Background { get; init; }

    /// <summary>
    /// The size of a tile in pixels. By default it is taken from the sheets (a sheet of B-E is 16 tiles wide); the sheets of a tileset must all agree. It must be a multiple of 4
    /// (a tile of an autotile is made of quarters, and the edge of a table is half of one).
    /// </summary>
    public int? TileSize { get; init; }
}
