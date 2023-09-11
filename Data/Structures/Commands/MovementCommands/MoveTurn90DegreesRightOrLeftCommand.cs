using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveTurn90DegreesRightOrLeftCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Turn90DegreesRightOrLeft;
        public override string Name => "Turn 90° Right or Left";
    }
}
