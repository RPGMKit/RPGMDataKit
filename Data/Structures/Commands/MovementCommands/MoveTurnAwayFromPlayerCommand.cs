using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveTurnAwayFromPlayerCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.TurnAwayFromPlayer;
        public override string GetCommandName(StorageVersion version) => "Turn away from Player";
    }
}
