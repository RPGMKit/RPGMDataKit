using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveThroughOffCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.ThroughOff;
        public override string Name => "Through OFF";
    }
}
