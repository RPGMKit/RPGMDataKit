using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveThroughOnCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.ThroughOn;
    public override string GetCommandName(StorageVersion version) => "Through ON";
}