using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveTurnRightCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.TurnRight;
    public override string GetCommandName(StorageVersion version) => "Turn Right";
}