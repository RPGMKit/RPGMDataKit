using System.Collections;
using System.Text;
using RPGMakerLibCoreV2.Data.VXAce.Classes;
using RubyMarshalCS;
using RubyMarshalCS.Conversion.Attributes;
using RubyMarshalCS.Conversion.Interfaces;
using RubyMarshalCS.Entities;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.SpecialTypes;

namespace RPGMakerLibCoreV2.Data.VXAce.Serializers;

[RubyCustomConverter]
public class ScriptConverter : IRubyCustomConverter
{
    public bool CanConvert(Type type)
    {
        return type == typeof(Script);
    }

    public AbstractEntity Serialize(object value, RubySerializer serializer)
    {
        var script = (Script)value;

        List<object> data = new();
        data.Add(script.Stamp);
        data.Add(new BinaryString(script.Name, !string.IsNullOrEmpty(script.Name) ? Encoding.UTF8 : null));

        BinaryString binaryString;
        if (string.IsNullOrEmpty(script.Code))
            binaryString = new BinaryString(RubyDeflate.Deflate([]), null);
        else
            binaryString = new BinaryString(RubyDeflate.Deflate(Encoding.UTF8.GetBytes(script.Code)),
                Encoding.UTF8);
        data.Add(binaryString);

        return serializer.Serialize(data);
    }

    public object Deserialize(AbstractEntity value, RubyDeserializer deserializer)
    {
        throw new Exception("Not implemented");
    }

    public object? Cast(object? o)
    {
        if (o == null)
            return null;
        
        if (!(o is IList))
            throw new Exception("Not a list");

        var list = (IList)o;

        var stamp = (int)list[0];
        var name = ((BinaryString)list[1]).Value;

        var codeString = (BinaryString)list[2];
        var enc = codeString.Encoding ?? Encoding.UTF8;

        return new Script()
        {
            Stamp = stamp,
            Name = name,
            Code = enc.GetString(RubyDeflate.Inflate(codeString.Bytes))
        };
    }
}
