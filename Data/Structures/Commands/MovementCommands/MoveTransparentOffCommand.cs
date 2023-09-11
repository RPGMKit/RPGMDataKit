using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveTransparentOffCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.TransparentOff;
        public override string Name => "Transparent OFF";
    }
}
