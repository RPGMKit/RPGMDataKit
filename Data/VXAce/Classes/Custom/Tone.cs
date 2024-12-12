using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.VXAce.Serializers;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.VXAce.Classes.Custom;

[RubyObject("Tone")]
[RubyUserSerializer(typeof(ToneSerializer), "")]
public class Tone : RubySerializationEntity, ITone
{
    [RubyProperty("@red")]
    public double _Red;

    [RubyProperty("@green")]
    public double _Green;

    [RubyProperty("@blue")]
    public double _Blue;

    [RubyProperty("@gray")]
    public double _Gray;

    public int Red
    {
        get => (int)_Red;
        set => _Red = value;
    }
    
    public int Green
    {
        get => (int)_Green;
        set => _Green = value;
    }
    
    public int Blue
    {
        get => (int)_Blue;
        set => _Blue = value;
    }
    
    public int Gray
    {
        get => (int)_Gray;
        set => _Gray = value;
    }
}
