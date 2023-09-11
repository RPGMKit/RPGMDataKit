using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveTurn90DegreesRightCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Turn90DegreesRight;
        public override string Name => "Turn 90° Right";
    }
}
