using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveDirectionFixOnCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.DirectionFixOn;
        public override string GetCommandName(StorageVersion version) => "Direction Fix ON";
    }
}
