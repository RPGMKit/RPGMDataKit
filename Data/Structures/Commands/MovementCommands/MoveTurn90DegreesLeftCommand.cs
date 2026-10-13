using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveTurn90DegreesLeftCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.Turn90DegreesLeft;
    public override string GetCommandName(StorageVersion version) => "Turn 90° Left";
}