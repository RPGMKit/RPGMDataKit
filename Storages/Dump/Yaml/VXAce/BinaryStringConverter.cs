using System.Text;
using RubyMarshalCS.SpecialTypes;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RPGMakerLibCoreV2.Storages.Dump.Yaml.VXAce;

class BinaryStringConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return typeof(BinaryString).IsAssignableFrom(type);
    }

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        parser.Consume<MappingStart>();

        Encoding enc = Encoding.UTF8;
        string value = "";

        while (parser.Current is Scalar)
        {
            var s = parser.Consume<Scalar>();
            switch (s.Value)
            {
                case "Encoding":
                    enc = rootDeserializer(typeof(Encoding)) as Encoding;
                    break;
                case "Value":
                    value = rootDeserializer(typeof(string)) as string;
                    break;
            }
        }

        parser.Consume<MappingEnd>();

        return new BinaryString(value, enc);
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        var b = (BinaryString)value;

        // text in UTF-8 is just a string; any other encoding is kept as it is
        if (b.Encoding?.CodePage == Encoding.UTF8.CodePage)
        {
            serializer(b.Value, typeof(string));
            return;
        }

        emitter.Emit(new MappingStart(null, "!BinaryString", false, MappingStyle.Block));

        emitter.Emit(new Scalar("Encoding"));
        serializer(b.Encoding);

        emitter.Emit(new Scalar("Value"));
        serializer(b.Value);

        emitter.Emit(new MappingEnd());
    }
}
