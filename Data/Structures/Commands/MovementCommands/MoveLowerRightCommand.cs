using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveLowerRightCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.MoveLowerRight;
    public override string GetCommandName(StorageVersion version) => "Move Lower Right";
}