using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IPage : ICommandContainer
{
    public string Dump(Storage storage, DumpOptions options);
}