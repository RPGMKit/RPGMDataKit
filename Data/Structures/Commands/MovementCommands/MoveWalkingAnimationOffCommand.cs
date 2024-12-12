using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveWalkingAnimationOffCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.WalkingAnimationOff;
    public override string GetCommandName(StorageVersion version) => "Walking Animation OFF";
}