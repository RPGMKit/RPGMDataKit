using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveOneStepBackwardCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.OneStepBackward;
        public override string Name => "1 Step Backward";
    }
}
