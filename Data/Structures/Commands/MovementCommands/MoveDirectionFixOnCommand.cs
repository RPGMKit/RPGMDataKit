using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveDirectionFixOnCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.DirectionFixOn;
    public override string GetCommandName(StorageVersion version) => "Direction Fix ON";
}