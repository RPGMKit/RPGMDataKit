using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ICommandContainer<out T> : IDumpable
{
    public List<NamedCommand> NamedCommands { get; set; }

    public IEnumerable<T>? Commands { get; }
}
