using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public class CommandContainer<T> : LookupList<T>, ICommandContainer where T : IEventCommand
{
    public IEnumerable<NamedCommand> NamedCommands => this.Select(_ => CommandFactory.Factory(_));

    public IEnumerable<IEventCommand> Commands => this.Select(_ => _ as IEventCommand);
}
