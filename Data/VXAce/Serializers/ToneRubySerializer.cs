using RPGMakerLibCoreV2.Data.VXAce.Classes.Custom;
using RubyMarshalCS.Serialization;

namespace RPGMakerLibCoreV2.Data.VXAce.Serializers;

class ToneSerializer : AbstractRubyUserSerializer<Tone>
{
    public override void Read(Tone obj, BinaryReader reader)
    {
        obj.Red = reader.ReadDouble();
        obj.Green = reader.ReadDouble();
        obj.Blue = reader.ReadDouble();
        obj.Gray = reader.ReadDouble();
    }

    public override void Write(Tone obj, BinaryWriter writer)
    {
        writer.Write((double)obj.Red);
        writer.Write((double)obj.Green);
        writer.Write((double)obj.Blue);
        writer.Write((double)obj.Gray);
    }
}
