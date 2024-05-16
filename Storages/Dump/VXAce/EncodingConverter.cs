using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RPGMakerLibCoreV2.Storages.Dump.VXAce;

class EncodingConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return typeof(Encoding).IsAssignableFrom(type);
    }

    public object? ReadYaml(IParser parser, Type type)
    {
        var scalar = parser.Consume<Scalar>();

        return Encoding.GetEncoding(Convert.ToInt32(scalar.Value));
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        emitter.Emit(new Scalar(((Encoding)value).CodePage.ToString()));
    }
}
