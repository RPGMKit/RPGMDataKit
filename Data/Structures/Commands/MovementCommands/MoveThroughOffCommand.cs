using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveThroughOffCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.ThroughOff;
    public override string GetCommandName(StorageVersion version) => "Through OFF";
}