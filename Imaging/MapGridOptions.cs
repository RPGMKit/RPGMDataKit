using SixLabors.Fonts;
using SixLabors.ImageSharp.PixelFormats;

namespace RPGMDataKit.Imaging;

/// <summary>What the squares of the grid say about the map.</summary>
public enum GridLabels
{
    /// <summary>Nothing.</summary>
    None,

    /// <summary>The place of the square in the map: <c>x,y</c> (the first square of the map is <c>0,0</c>, also when a part of the map is drawn).</summary>
    Coordinates,

    /// <summary>The region id of the square (0, no region, is not written).</summary>
    Region,

    /// <summary>The id of the tile on the layer <see cref="MapGridOptions.TileLayer"/> (0, no tile, is not written).</summary>
    TileId,
}

/// <summary>Where the text of a square stands.</summary>
public enum GridLabelPosition
{
    TopLeft,
    Center,
}

/// <summary>The grid over the picture of a map: a line between the squares, and a text in each of them if it is asked for.</summary>
public sealed class MapGridOptions
{
    /// <summary>The colour of the light line (white with half of the alpha by default). Every line is two: this one and <see cref="ShadowColor"/> beside it, so one of them is seen on a light map and on a dark one.</summary>
    public Rgba32 Color { get; init; } = new(255, 255, 255, 140);

    /// <summary>The colour of the dark line that lies next to the light one (black with half of the alpha by default); transparent for a line of one colour.</summary>
    public Rgba32 ShadowColor { get; init; } = new(0, 0, 0, 140);

    /// <summary>
    /// The width of each of the two lines in pixels, from 1 to a quarter of a tile. A light line is at the left and the top of every square and at the right and the bottom of the picture, the dark one
    /// is next to it on the side of the square. Where a light line and a dark one cross, the light one is on top.
    /// </summary>
    public int LineWidth { get; init; } = 1;

    /// <summary>What is written in the squares. Nothing by default.</summary>
    public GridLabels Labels { get; init; } = GridLabels.None;

    /// <summary>
    /// The layer of tiles whose id <see cref="GridLabels.TileId"/> writes: 0 is the lowest, up to 2 in VX Ace and 3 in MV (<c>TileLayerCount() - 1</c>; the shadows and regions are not layers of tiles). 0 by default.
    /// A layer the map does not have is read as no tile, and nothing is written.
    /// </summary>
    /// <remarks>Used only by <see cref="GridLabels.TileId"/>.</remarks>
    public int TileLayer { get; init; }

    /// <summary>
    /// The text of a square of your own, in place of <see cref="Labels"/>: it gets the place in the map (x, y) and returns the text, or null (or an empty text) for no text.
    /// </summary>
    public Func<int, int, string?>? Label { get; init; }

    /// <summary>Where the text stands in the square. The top left corner by default.</summary>
    public GridLabelPosition LabelPosition { get; init; } = GridLabelPosition.TopLeft;

    /// <summary>The colour of the text.</summary>
    public Rgba32 LabelColor { get; init; } = new(255, 255, 255, 255);

    /// <summary>The colour of the outline of the text (one pixel all around it), so that it can be read on light tiles and on dark ones; transparent for none.</summary>
    public Rgba32 LabelOutline { get; init; } = new(0, 0, 0, 220);

    /// <summary>
    /// The font of the text. When it is not given one is taken from the system (Arial, Segoe UI, DejaVu Sans, Liberation Sans, or the first there is); a machine with no fonts must give one.
    /// The size follows the size of a tile and is made smaller when a text does not fit in the square.
    /// </summary>
    public FontFamily? FontFamily { get; init; }

    internal bool HasLabels => Label != null || Labels != GridLabels.None;
}
