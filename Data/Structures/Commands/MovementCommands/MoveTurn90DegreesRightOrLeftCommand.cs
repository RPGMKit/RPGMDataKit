using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveTurn90DegreesRightOrLeftCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Turn90DegreesRightOrLeft;
        public override string GetCommandName(StorageVersion version) => "Turn 90° Right or Left";
    }
}
