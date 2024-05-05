using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2;

public interface IDumpable
{
    public string Dump(Storage storage, DumpOptions options);
}