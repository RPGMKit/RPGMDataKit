using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveDownCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.MoveDown;
    public override string GetCommandName(StorageVersion version) => "Move Down";
}