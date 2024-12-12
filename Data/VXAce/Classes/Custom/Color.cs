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
    
    public int Alpha
    {
        get => (int)_Alpha;
        set => _Alpha = value;
    }
}
