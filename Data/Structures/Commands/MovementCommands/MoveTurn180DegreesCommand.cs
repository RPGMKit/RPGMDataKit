using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveTurn180DegreesCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Turn180Degrees;
        public override string Name => "Turn 180°";
    }

}
