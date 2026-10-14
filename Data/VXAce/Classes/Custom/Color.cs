using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.VXAce.Serializers;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes.Custom;

[RubyObject("Color")]
[RubyUserSerializer(typeof(ColorSerializer))]
public class Color : IColor, INewEntity
{
    /// <summary>An opaque white (255, 255, 255, 255).</summary>
    public void InitializeNew()
    {
        Red = 255;
        Green = 255;
        Blue = 255;
        Alpha = 255;
    }

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
