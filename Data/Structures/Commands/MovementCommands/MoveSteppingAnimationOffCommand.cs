using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveSteppingAnimationOffCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.SteppingAnimationOff;
    public override string GetCommandName(StorageVersion version) => "Stepping Animation OFF";
}