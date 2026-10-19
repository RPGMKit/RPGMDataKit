using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RPGMDataKit.Imaging;

/// <summary>A picture that is drawn on: pixels laid over one another the way a canvas does it (the new pixel over the old one by its alpha).</summary>
internal sealed class PixelCanvas
{
    private readonly Rgba32[] _pixels;

    public PixelCanvas(int width, int height, Rgba32 background = default)
    {
        Width = width;
        Height = height;
        _pixels = new Rgba32[width * height];
        if (background != default)
            Array.Fill(_pixels, background);
    }

    public int Width { get; }

    public int Height { get; }

    public Rgba32[] Pixels => _pixels;

    /// <summary>Copies a rectangle of the sheet onto the canvas, over what is there. A part that is outside of the sheet or of the canvas is left out.</summary>
    public void Draw(TilesetSheet source, int sx, int sy, int width, int height, int dx, int dy)
    {
        for (var y = 0; y < height; y++)
        {
            var from = sy + y;
            var to = dy + y;
            if ((uint)from >= (uint)source.Height || (uint)to >= (uint)Height)
                continue;

            for (var x = 0; x < width; x++)
            {
                var fromX = sx + x;
                var toX = dx + x;
                if ((uint)fromX >= (uint)source.Width || (uint)toX >= (uint)Width)
                    continue;

                Over(ref _pixels[to * Width + toX], source.Pixels[from * source.Width + fromX]);
            }
        }
    }

    /// <summary>Lays a colour over a rectangle (the shadows are black with the alpha of a half).</summary>
    public void Fill(int dx, int dy, int width, int height, Rgba32 color)
    {
        for (var y = 0; y < height; y++)
        {
            var to = dy + y;
            if ((uint)to >= (uint)Height)
                continue;

            for (var x = 0; x < width; x++)
            {
                var toX = dx + x;
                if ((uint)toX < (uint)Width)
                    Over(ref _pixels[to * Width + toX], color);
            }
        }
    }

    /// <summary>Lays another canvas of the same size over this one.</summary>
    public void DrawCanvas(PixelCanvas other)
    {
        for (var i = 0; i < _pixels.Length; i++)
            Over(ref _pixels[i], other._pixels[i]);
    }

    public Image<Rgba32> ToImage() => Image.LoadPixelData<Rgba32>(_pixels, Width, Height);

    // the colour s laid over d: the alpha adds up, the colour is the mean of the two by their alphas; whole numbers, rounded
    internal static void Over(ref Rgba32 d, Rgba32 s)
    {
        if (s.A == 0)
            return;
        if (s.A == 255 || d.A == 0)
        {
            d = s;
            return;
        }

        var sa = (int)s.A;
        var da = (int)d.A;
        var total = sa * 255 + da * (255 - sa);   // the alpha out, times 255
        var half = total / 2;
        d.R = (byte)((s.R * sa * 255 + d.R * da * (255 - sa) + half) / total);
        d.G = (byte)((s.G * sa * 255 + d.G * da * (255 - sa) + half) / total);
        d.B = (byte)((s.B * sa * 255 + d.B * da * (255 - sa) + half) / total);
        d.A = (byte)((total + 127) / 255);
    }
}
