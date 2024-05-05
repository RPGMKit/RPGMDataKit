using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveTurnDownCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.TurnDown;
        public override string GetCommandName(StorageVersion version) => "Turn Down";
    }
}
