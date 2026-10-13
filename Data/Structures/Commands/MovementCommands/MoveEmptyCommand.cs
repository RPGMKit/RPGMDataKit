using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveEmptyCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.End;
    public override string GetCommandName(StorageVersion version) => "Empty";
}