using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveLowerRightCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.LowerRight;
        public override string Name => "Move Lower Right";
    }
}
