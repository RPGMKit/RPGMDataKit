using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveDirectionFixOffCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.DirectionFixOff;
    public override string GetCommandName(StorageVersion version) => "Direction Fix OFF";
}