using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveTurnLeftCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.TurnLeft;
    public override string GetCommandName(StorageVersion version) => "Turn Left";
}