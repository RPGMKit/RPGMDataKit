using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveTransparentOffCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.TransparentOff;
    public override string GetCommandName(StorageVersion version) => "Transparent OFF";
}