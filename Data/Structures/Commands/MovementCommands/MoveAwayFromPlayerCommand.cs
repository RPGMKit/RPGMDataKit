using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveAwayFromPlayerCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.AwayFromPlayer;
    public override string GetCommandName(StorageVersion version) => "Move away from Player";
}