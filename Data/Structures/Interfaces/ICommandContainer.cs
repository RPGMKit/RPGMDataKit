using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ICommandContainer : IDumpable
{
    public IEnumerable<NamedCommand> NamedCommands { get; }

    public IEnumerable<IEventCommand> Commands { get; }
}
