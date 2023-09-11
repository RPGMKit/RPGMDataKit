using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveAtRandomCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.AtRandom;
        public override string Name => "Move at Random";
    }
}
