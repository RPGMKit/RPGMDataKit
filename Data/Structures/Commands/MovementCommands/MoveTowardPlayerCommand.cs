using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveTowardPlayerCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.MoveTowardPlayer;
    public override string GetCommandName(StorageVersion version) => "Move toward Player";
}