using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveRightCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Right;
        public override string GetCommandName(StorageVersion version) => "Move Right";
    }
}
