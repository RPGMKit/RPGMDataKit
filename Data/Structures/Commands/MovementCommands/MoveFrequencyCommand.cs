using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveFrequencyCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Frequency;
        public override string Name => "Frequency";
        
        public int GetFrequency()
        {
            return GetParameter<int>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return GetFrequency().ToString();
        }
    }
}
