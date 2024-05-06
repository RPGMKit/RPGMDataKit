using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.VXAce.Serializers;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes.Custom;

[RubyObject("Color")]
[RubyUserSerializer(typeof(ColorSerializer))]
public class Color : IColor
{
    [RubyProperty("@red")]
    public double _Red;

    [RubyProperty("@green")]
    public double _Green;

    [RubyProperty("@blue")]
    public double _Blue;

    [RubyProperty("@alpha")]
    public double _Alpha;

    public int Red => (int)_Red;
    public int Green => (int)_Green;
    public int Blue => (int)_Blue;
    public int Alpha => (int)_Alpha;
}
