using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveTransparentOnCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.TransparentOn;
        public override string Name => "Transparent ON";
    }
}
