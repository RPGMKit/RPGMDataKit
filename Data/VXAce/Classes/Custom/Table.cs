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
}
