using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IPage : ICommandContainer
{
    public string Dump(Storage storage, DumpOptions options);
}