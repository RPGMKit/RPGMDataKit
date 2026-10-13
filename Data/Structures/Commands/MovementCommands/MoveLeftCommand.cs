using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveLeftCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.MoveLeft;
    public override string GetCommandName(StorageVersion version) => "Move Left";
}