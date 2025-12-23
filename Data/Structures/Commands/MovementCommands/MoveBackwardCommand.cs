using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveBackwardCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.MoveBackward;
    public override string GetCommandName(StorageVersion version) => "1 Step Backward";
}