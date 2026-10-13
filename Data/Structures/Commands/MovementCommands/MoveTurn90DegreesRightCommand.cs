using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveTurn90DegreesRightCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.Turn90DegreesRight;
    public override string GetCommandName(StorageVersion version) => "Turn 90° Right";
}