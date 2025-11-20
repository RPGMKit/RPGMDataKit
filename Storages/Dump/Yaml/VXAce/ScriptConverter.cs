using System.Text.RegularExpressions;
using RPGMakerLibCoreV2.Data.VXAce.Classes;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.SpecialTypes;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using Scalar = YamlDotNet.Core.Events.Scalar;

namespace RPGMakerLibCoreV2.Storages.Dump.Yaml.VXAce;

class ScriptConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return typeof(Script).IsAssignableFrom(type);
    }

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer deserializer)
    {
        parser.Consume<MappingStart>();

        Script scr = new();
        scr.Grow(3);
        
        while (parser.Current is Scalar)
        {
            var s = parser.Consume<Scalar>();
            switch (s.Value)
            {
                case nameof(scr.Stamp):
                    scr.Stamp = (int)deserializer(typeof(int));
                    break;
                case nameof(scr.Name):
                    scr[1] = deserializer(typeof(BinaryString));
                    break;
                case nameof(scr.Code):
                    var str = deserializer(typeof(BinaryString)) as BinaryString;
                    str.Value = Regex.Replace(str.Value, "^#", "", RegexOptions.Multiline);
                    var tmp = new BinaryString(RubyDeflate.Deflate(str.Bytes), str.Encoding);
                    scr[2] = tmp;
                    break;
            }
        }

        var code = scr.Code;

        parser.Consume<MappingEnd>();

        return scr;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        var b = (Script)value;

        emitter.Emit(new MappingStart(null, "!Script", false, MappingStyle.Block));

        emitter.Emit(new Scalar(nameof(b.Stamp)));
        serializer(b.Stamp);

        emitter.Emit(new Scalar(nameof(b.Name)));
        serializer(b[1]);

        var tmp = new BinaryString(Regex.Replace(b.Code, "^", "#", RegexOptions.Multiline), (b[2] as BinaryString).Encoding);
        emitter.Emit(new Scalar(nameof(b.Code)));
        serializer(tmp);
        // emitter.Emit(new Scalar(AnchorName.Empty, null, b.Code.Replace("\n", "\n#"), ScalarStyle.Literal, true, false));

        emitter.Emit(new MappingEnd());
    }
}
