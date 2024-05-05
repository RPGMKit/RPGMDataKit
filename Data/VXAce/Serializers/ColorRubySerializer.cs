using RPGMakerLibCoreV2.Data.VXAce.Classes.Custom;
using RubyMarshalCS.Serialization;

namespace RPGMakerLibCoreV2.Data.VXAce.Serializers;

class ColorSerializer : AbstractRubyUserSerializer<Color>
{
    public override void Read(Color obj, BinaryReader reader)
    {
        obj.Red = reader.ReadDouble();
        obj.Green = reader.ReadDouble();
        obj.Blue = reader.ReadDouble();
        obj.Alpha = reader.ReadDouble();
    }

    public override void Write(Color obj, BinaryWriter writer)
    {
        writer.Write((double)obj.Red);
        writer.Write((double)obj.Green);
        writer.Write((double)obj.Blue);
        writer.Write((double)obj.Alpha);
    }
}
