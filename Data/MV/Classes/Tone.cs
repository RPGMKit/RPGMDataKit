using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

/// <summary>A tone as MV stores it: an array of red, green, blue and gray.</summary>
public class Tone : List<int>, ITone, INewEntity
{
    /// <summary>A tone of four zeros, as in a new "Tint Screen" command of the editor (no change of the colors); the array of MV has to have all four numbers.</summary>
    public void InitializeNew()
    {
        Clear();
        AddRange([0, 0, 0, 0]);
    }

    public static Tone FromArray(IList<int> list)
    {
        return new Tone()
        {
            Red = list[0],
            Green = list[1],
            Blue = list[2],
            Gray = list[3],
        };
    }

    public int Red
    {
        get => this.Get(0);
        set => this.Set(0, value);
    }

    public int Green
    {
        get => this.Get(1);
        set => this.Set(1, value);
    }

    public int Blue
    {
        get => this.Get(2);
        set => this.Set(2, value);
    }

    public int Gray
    {
        get => this.Get(3);
        set => this.Set(3, value);
    }
}
