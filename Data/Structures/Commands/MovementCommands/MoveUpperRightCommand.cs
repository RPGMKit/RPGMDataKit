using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveUpperRightCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.MoveUpperRight;
    public override string GetCommandName(StorageVersion version) => "Move Upper Right";
}