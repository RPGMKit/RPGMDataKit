using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveThroughOnCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.ThroughOn;
    public override string GetCommandName(StorageVersion version) => "Through ON";
}