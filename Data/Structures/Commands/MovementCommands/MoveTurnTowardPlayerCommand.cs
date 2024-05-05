using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveTurnTowardPlayerCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.TurnTowardPlayer;
        public override string GetCommandName(StorageVersion version) => "Turn toward Player";
    }
}
