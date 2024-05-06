using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class Tone : ITone
{
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

    public int Red { get; set; }
    public int Green { get; set; }
    public int Blue { get; set; }
    public int Gray { get; set; }
}
