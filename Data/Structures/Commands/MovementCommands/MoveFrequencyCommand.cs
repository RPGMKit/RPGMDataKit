using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveFrequencyCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Frequency;
        public override string GetCommandName(StorageVersion version) => "Frequency";
        
        public int GetFrequency()
        {
            return GetParameter<int>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return GetFrequency().ToString();
        }
    }
}
