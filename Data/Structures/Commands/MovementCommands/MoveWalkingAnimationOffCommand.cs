using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveWalkingAnimationOffCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.WalkingAnimationOff;
    public override string GetCommandName(StorageVersion version) => "Walking Animation OFF";
}