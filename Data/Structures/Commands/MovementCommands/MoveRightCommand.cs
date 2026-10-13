using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveRightCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.MoveRight;
    public override string GetCommandName(StorageVersion version) => "Move Right";
}