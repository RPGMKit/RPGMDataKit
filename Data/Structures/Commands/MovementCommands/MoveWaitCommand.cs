using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveWaitCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Wait;
        public override string Name => "Wait";

        public int GetFrames()
        {
            return GetParameter<int>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return $"{GetFrames()} frame(s)";
        }
    }
}
