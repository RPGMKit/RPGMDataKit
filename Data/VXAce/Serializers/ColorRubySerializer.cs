using RPGMakerLibCoreV2.Data.VXAce.Classes.Custom;
using RubyMarshalCS.Serialization.Interfaces;

namespace RPGMakerLibCoreV2.Data.VXAce.Serializers;

class ColorSerializer : IRubyUserSerializer<Color>
{
    public void Read(Color obj, BinaryReader reader)
    {
        obj._Red = reader.ReadDouble();
        obj._Green = reader.ReadDouble();
        obj._Blue = reader.ReadDouble();
        obj._Alpha = reader.ReadDouble();
    }

    public void Write(Color obj, BinaryWriter writer)
    {
        writer.Write((double)obj._Red);
        writer.Write((double)obj._Green);
        writer.Write((double)obj._Blue);
        writer.Write((double)obj._Alpha);
    }
}
