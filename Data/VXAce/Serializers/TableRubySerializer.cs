using RPGMakerLibCoreV2.Data.Structures;
using RubyMarshalCS.Serialization.Interfaces;

namespace RPGMakerLibCoreV2.Data.VXAce.Serializers;

class TableSerializer : IRubyUserSerializer<Table>
{
    public void Read(Table obj, BinaryReader reader)
    {
        obj.NumOfDimensions = reader.ReadInt32();
        obj.SizeX = reader.ReadInt32();
        obj.SizeY = reader.ReadInt32();
        obj.SizeZ = reader.ReadInt32();
        obj.NumOfElements = reader.ReadInt32();

        obj.Elements = new();
        for (var i = 0; i < obj.NumOfElements; ++i)
            obj.Elements.Add(reader.ReadInt16());

        var p = reader.BaseStream.Position;
    }

    public void Write(Table obj, BinaryWriter writer)
    {
        writer.Write((int)obj.NumOfDimensions);
        writer.Write((int)obj.SizeX);
        writer.Write((int)obj.SizeY);
        writer.Write((int)obj.SizeZ);
        writer.Write(obj.Elements.Count);
        foreach (var element in obj.Elements)
            writer.Write(checked((short)element));

        var p = writer.BaseStream.Position;
    }
}
