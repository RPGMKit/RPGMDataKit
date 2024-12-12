using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveTowardPlayerCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.TowardPlayer;
    public override string GetCommandName(StorageVersion version) => "Move toward Player";
}