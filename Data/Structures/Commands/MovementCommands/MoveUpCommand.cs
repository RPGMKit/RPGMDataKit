using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveUpCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.MoveUp;
    public override string GetCommandName(StorageVersion version) => "Move Up";
}