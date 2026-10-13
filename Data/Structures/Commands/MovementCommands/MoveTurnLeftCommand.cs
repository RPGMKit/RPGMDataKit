using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveTurnLeftCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.TurnLeft;
    public override string GetCommandName(StorageVersion version) => "Turn Left";
}