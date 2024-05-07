using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveOneStepBackwardCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.OneStepBackward;
        public override string GetCommandName(StorageVersion version) => "1 Step Backward";
    }
}
