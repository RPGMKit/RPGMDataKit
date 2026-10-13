using RPGMDataKit.Data.VXAce.Classes.Custom;
using RubyMarshalCS.Serialization.Interfaces;

namespace RPGMDataKit.Data.VXAce.Serializers;

class ToneSerializer : IRubyUserSerializer<Tone>
{
    public void Read(Tone obj, BinaryReader reader)
    {
        obj._Red = reader.ReadDouble();
        obj._Green = reader.ReadDouble();
        obj._Blue = reader.ReadDouble();
        obj._Gray = reader.ReadDouble();
    }

    public void Write(Tone obj, BinaryWriter writer)
    {
        writer.Write((double)obj._Red);
        writer.Write((double)obj._Green);
        writer.Write((double)obj._Blue);
        writer.Write((double)obj._Gray);
    }
}
