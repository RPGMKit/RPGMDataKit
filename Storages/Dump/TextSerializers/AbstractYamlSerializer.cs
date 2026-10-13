using YamlDotNet.Serialization;

namespace RPGMDataKit.Storages.Dump.TextSerializers;

public abstract class AbstractYamlSerializer : IToTextSerializer
{
    protected abstract ISerializer CreateSerializer();

    protected abstract IDeserializer CreateDeserializer();

    public virtual string Serialize<T>(T obj) => CreateSerializer().Serialize(obj);

    public virtual T Deserialize<T>(string text) => CreateDeserializer().Deserialize<T>(text);
}
