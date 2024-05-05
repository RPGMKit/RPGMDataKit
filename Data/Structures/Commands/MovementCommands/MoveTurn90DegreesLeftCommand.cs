using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveTurn90DegreesLeftCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Turn90DegreesLeft;
        public override string GetCommandName(StorageVersion version) => "Turn 90° Left";
    }
}
