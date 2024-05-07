using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveWaitCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Wait;
        public override string GetCommandName(StorageVersion version) => "Wait";

        public int GetFrames()
        {
            return GetParameter<int>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return $"{GetFrames()} frame(s)";
        }
    }
}
