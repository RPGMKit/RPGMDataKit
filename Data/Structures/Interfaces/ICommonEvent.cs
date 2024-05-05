using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ICommonEvent<T> where T : IEventCommand
{
    public int Id { get; set; }

    public string Name { get; set; }

    public CommandContainer<T> List { get; set; }

    public string Dump(Storage storage, DumpOptions options);
}