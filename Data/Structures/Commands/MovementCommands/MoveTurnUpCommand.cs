using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveTurnUpCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.TurnUp;
        public override string GetCommandName(StorageVersion version) => "Turn Up";
    }
}
