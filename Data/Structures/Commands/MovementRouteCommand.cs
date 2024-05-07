using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories.MovementCommands;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using MoveCommandMV = RPGMakerLibCoreV2.Data.MV.Classes.MoveCommand;

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
            IMoveCommand eventCommand = version == StorageVersion.MV
                ? GetParameter<MoveCommandMV>(0)
                : GetParameter<IMoveCommand>(0);

            return MovementCommandFactory.Create(eventCommand);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var moveCommand = Factory(context.Version);

            return moveCommand.AsString(context, new DumpOptions() {AppendCommandCode = options.AppendCommandCode, WithIndent = false, EmphasiseStrings = options.EmphasiseStrings});
        }
    }
}
