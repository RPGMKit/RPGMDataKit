using RPGMDataKit.Storages.Dump.TextSerializers;

namespace RPGMDataKit.Storages.Dump;

public abstract class AbstractYamlStorage(IToTextSerializer serializer) : AbstractTextStorage(serializer)
{
    protected override string GetExtension<T>() => "yaml";
}
