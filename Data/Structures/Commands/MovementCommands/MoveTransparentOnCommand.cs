using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveTransparentOnCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.TransparentOn;
    public override string GetCommandName(StorageVersion version) => "Transparent ON";
}