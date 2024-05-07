using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveOpacityCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Opacity;
        public override string GetCommandName(StorageVersion version) => "Opacity";

        public int GetOpacity()
        {
            return GetParameter<int>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return GetOpacity().ToString();
        }
    }
}
