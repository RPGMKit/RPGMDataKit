using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class LoopCommand : NamedCommand
    {
        public override int Code => CommandCodes.Loop;
        public override string Name => "Loop";
    }
}
