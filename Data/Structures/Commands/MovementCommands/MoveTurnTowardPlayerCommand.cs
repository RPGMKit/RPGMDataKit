using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveTurnTowardPlayerCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.TurnTowardPlayer;
    public override string GetCommandName(StorageVersion version) => "Turn toward Player";
}