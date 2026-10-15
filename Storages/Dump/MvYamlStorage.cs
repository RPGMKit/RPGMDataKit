using RPGMDataKit.Storages.Abstraction;
using RPGMDataKit.Storages.Dump.TextSerializers;

namespace RPGMDataKit.Storages.Dump;

internal sealed class MvYamlStorage : AbstractYamlStorage
{
    public MvYamlStorage(Storage storage) : base(storage, new MvYamlSerializer(storage.Types))
    {
    }
}
