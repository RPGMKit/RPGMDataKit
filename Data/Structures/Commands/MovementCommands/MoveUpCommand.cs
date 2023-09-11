using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveUpCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Up;
        public override string Name => "Move Up";
    }
}
