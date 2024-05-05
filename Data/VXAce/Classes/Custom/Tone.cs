using RPGMakerLibCoreV2.Data.VXAce.Serializers;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes.Custom;

[RubyObject("Tone")]
[RubyUserSerializer(typeof(ToneSerializer), "")]
class Tone : RubySerializationEntity
{
    [RubyProperty("@red")]
    public double Red;

    [RubyProperty("@green")]
    public double Green;

    [RubyProperty("@blue")]
    public double Blue;

    [RubyProperty("@gray")]
    public double Gray;
}
