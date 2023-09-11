using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveLowerLeftCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.LowerLeft;
        public override string Name => "Move Lower Left";
    }
}
