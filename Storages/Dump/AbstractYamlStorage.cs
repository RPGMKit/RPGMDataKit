using RPGMDataKit.Storages.Abstraction;
using RPGMDataKit.Storages.Dump.TextSerializers;

namespace RPGMDataKit.Storages.Dump;

public abstract class AbstractYamlStorage : AbstractTextStorage
{
    internal AbstractYamlStorage(Storage storage, IToTextSerializer serializer) : base(storage, serializer)
    {
    }

    protected override string GetExtension<T>() => "yaml";
}
