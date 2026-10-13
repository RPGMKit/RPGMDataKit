using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveUpperLeftCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.MoveUpperLeft;
    public override string GetCommandName(StorageVersion version) => "Move Upper Left";
}