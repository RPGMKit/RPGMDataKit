using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class Color : List<int>, IColor, INewEntity
{
    /// <summary>An opaque white (255, 255, 255, 255); the array of MV has to have all four numbers.</summary>
    public void InitializeNew()
    {
        Clear();
        AddRange([255, 255, 255, 255]);
    }

    public static Color FromArray(IList<int> list)
    {
        return new Color()
        {
            Red = list[0],
            Green = list[1],
            Blue = list[2],
            Alpha = list[3],
        };
    }

    public int Red
    {
        get => this[0]; set => this.Set(0, value);
    }
    public int Green
    {
        get => this[1]; set => this.Set(1, value);
    }
    public int Blue
    {
        get => this[2]; set => this.Set(2, value);
    }
    public int Alpha
    {
        get => this[3]; set => this.Set(3, value);
    }
}
