using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveDownCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Down;
        public override string GetCommandName(StorageVersion version) => "Move Down";
    }
}
