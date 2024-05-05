using RPGMakerLibCoreV2.Data.VXAce.Serializers;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes.Custom;

[RubyObject("Color")]
[RubyUserSerializer(typeof(ColorSerializer))]
public class Color
{
    [RubyProperty("@red")]
    public double Red;
    [RubyProperty("@green")]
    public double Green;
    [RubyProperty("@blue")]
    public double Blue;
    [RubyProperty("@alpha")]
    public double Alpha;
}
