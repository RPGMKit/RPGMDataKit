using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveOneStepForwardCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.OneStepForward;
        public override string Name => "1 Step Forward";
    }
}
