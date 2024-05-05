using RPGMakerLibCoreV2.Data.VXAce.Classes.Custom;
using RubyMarshalCS.Serialization;

namespace RPGMakerLibCoreV2.Data.VXAce.Serializers;

class TableSerializer : AbstractRubyUserSerializer<Table>
{
    public override void Read(Table obj, BinaryReader reader)
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

    public override void Write(Table obj, BinaryWriter writer)
    {
        writer.Write((int)obj.NumOfDimensions);
        writer.Write((int)obj.SizeX);
        writer.Write((int)obj.SizeY);
        writer.Write((int)obj.SizeZ);
        writer.Write((int)obj.NumOfElements);
        for (var i = 0; i < obj.NumOfElements; ++i)
            writer.Write((short)obj.Elements[i]);

        var p = writer.BaseStream.Position;
    }
}
