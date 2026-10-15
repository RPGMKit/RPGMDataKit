using RPGMDataKit.Data.VXAce.Classes;
using RPGMDataKit.Storages.Abstraction;
using RPGMDataKit.Storages.Dump.TextSerializers;

namespace RPGMDataKit.Storages.Dump;

internal sealed class VxAceYamlStorage : AbstractYamlStorage
{
    public VxAceYamlStorage(Storage storage) : base(storage, new VxAceYamlSerializer(storage.Types))
    {
        AddSpecialSerializer<Script>(new ScriptSerializer());
    }

    protected override string GetExtension<T>()
    {
        if (typeof(T) == typeof(Script))
            return "txt";
        return base.GetExtension<T>();
    }
}
