using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories.MovementCommands;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;
using EventCommandMV = RPGMakerLibCoreV2.Data.MV.Classes.EventCommand;
using EventCommandVXAce = RPGMakerLibCoreV2.Data.VXAce.Classes.EventCommand;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class MovementRouteCommand : NamedCommand
    {
        public MovementRouteCommand()
        {
            IsSecondary = true;
        }

        public override int Code => CommandCodes.MovementRouteCommand;
        public override string GetCommandName(StorageVersion version) => "                  ";

        public NamedCommand Factory(StorageVersion version)
        {
            IEventCommand eventCommand = version == StorageVersion.MV
                ? GetParameter<EventCommandMV>(0)
                : GetParameter<EventCommandVXAce>(0);

            return MovementCommandFactory.Create(eventCommand);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var moveCommand = Factory(storage.Version);

            return moveCommand.AsString(storage, new DumpOptions() {AppendCommandCode = options.AppendCommandCode, WithIndent = false, EmphasiseStrings = options.EmphasiseStrings});
        }
    }
}
