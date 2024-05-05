using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveUpperRightCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.UpperRight;
        public override string GetCommandName(StorageVersion version) => "Move Upper Right";
    }
}
