using RPGMakerLibCoreV2.Data.VXAce.Classes.Custom;
using RubyMarshalCS.Serialization;

namespace RPGMakerLibCoreV2.Data.VXAce.Serializers;

class ToneSerializer : AbstractRubyUserSerializer<Tone>
{
    public override void Read(Tone obj, BinaryReader reader)
    {
        obj._Red = reader.ReadDouble();
        obj._Green = reader.ReadDouble();
        obj._Blue = reader.ReadDouble();
        obj._Gray = reader.ReadDouble();
    }

    public override void Write(Tone obj, BinaryWriter writer)
    {
        writer.Write((double)obj._Red);
        writer.Write((double)obj._Green);
        writer.Write((double)obj._Blue);
        writer.Write((double)obj._Gray);
    }
}
