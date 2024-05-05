using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveTurnRightCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.TurnRight;
        public override string GetCommandName(StorageVersion version) => "Turn Right";
    }
}
