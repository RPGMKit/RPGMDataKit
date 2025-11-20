using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RPGMakerLibCoreV2.Storages.Dump.Yaml.VXAce;

class EncodingConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return typeof(Encoding).IsAssignableFrom(type);
    }

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        var scalar = parser.Consume<Scalar>();
        if (string.IsNullOrWhiteSpace(scalar.Value))
            return null;

        return Encoding.GetEncoding(Convert.ToInt32(scalar.Value));
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value == null)
            serializer(null);
        
        serializer(((Encoding)value).CodePage);
    }
}
