using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveLeftCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Left;
        public override string GetCommandName(StorageVersion version) => "Move Left";
    }
}
