using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.VXAce.Serializers;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes.Custom;

[RubyObject("Tone")]
[RubyUserSerializer(typeof(ToneSerializer), "")]
class Tone : RubySerializationEntity, ITone
{
    [RubyProperty("@red")]
    public double _Red;

    [RubyProperty("@green")]
    public double _Green;

    [RubyProperty("@blue")]
    public double _Blue;

    [RubyProperty("@gray")]
    public double _Gray;

    public int Red => (int)_Red;
    public int Green => (int)_Green;
    public int Blue => (int)_Blue;
    public int Gray => (int)_Gray;
}
