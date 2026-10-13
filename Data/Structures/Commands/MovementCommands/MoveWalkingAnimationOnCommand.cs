using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveWalkingAnimationOnCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.WalkingAnimationOn;
    public override string GetCommandName(StorageVersion version) => "Walking Animation ON";
}