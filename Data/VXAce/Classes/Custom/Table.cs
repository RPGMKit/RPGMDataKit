using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.VXAce.Serializers;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes.Custom;

[RubyObject("Table")]
[RubyUserSerializer(typeof(TableSerializer), "")]
public class Table : RubySerializationEntity
{
    [RubyProperty("@num_of_dimensions")]
    public int NumOfDimensions;

    [RubyProperty("@xsize")]
    public int SizeX;

    [RubyProperty("@ysize")]
    public int SizeY;
    
    [RubyProperty("@zsize")]
    public int SizeZ;

    [RubyProperty("@num_of_elements")]
    public int NumOfElements;

    [RubyProperty("@elements")]
    public List<short> Elements;

    public static Table Create3D(int sizeX, int sizeY, IReadOnlyCollection<int> elements)
    {
        var cells = sizeX * sizeY;

        return new Table
        {
            NumOfDimensions = 3,
            SizeX = sizeX,
            SizeY = sizeY,
            SizeZ = cells > 0 ? elements.Count / cells : 0,
            NumOfElements = elements.Count,
            Elements = elements.Select(e => (short)e).ToList()
        };
    }

    public List<int> ToInts() => Elements?.Select(e => (int)e).ToList() ?? new();
}
