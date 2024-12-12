using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Color : JsonEntity, IColor
{
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

    public int Red { get; set; }
    public int Green { get; set; }
    public int Blue { get; set; }
    public int Alpha { get; set; }
}
