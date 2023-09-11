using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ExitEventProcessingCommand : NamedCommand
    {
        public override int Code => CommandCodes.ExitEventProcessing;
        public override string Name => "Exit Event Processing";
    }
}
