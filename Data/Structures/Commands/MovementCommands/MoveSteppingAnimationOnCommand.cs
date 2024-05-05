using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveSteppingAnimationOnCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.SteppingAnimationOn;
        public override string GetCommandName(StorageVersion version) => "Stepping Animation ON";
    }
}
