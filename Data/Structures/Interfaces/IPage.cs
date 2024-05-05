using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IPage<T> where T : IEventCommand
{
    public CommandContainer<T> List { get; set; }
    
    public string Dump(Storage storage, DumpOptions options);
}