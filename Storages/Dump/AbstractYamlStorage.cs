using RPGMakerLibCoreV2.Storages.Dump.TextSerializers;

namespace RPGMakerLibCoreV2.Storages.Dump;

public abstract class AbstractYamlStorage(IToTextSerializer serializer) : AbstractTextStorage(serializer)
{
    protected override string GetExtension<T>() => "yaml";
}
