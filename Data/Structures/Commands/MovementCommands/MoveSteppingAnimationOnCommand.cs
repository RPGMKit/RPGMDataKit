using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveSteppingAnimationOnCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.SteppingAnimationOn;
    public override string GetCommandName(StorageVersion version) => "Stepping Animation ON";
}