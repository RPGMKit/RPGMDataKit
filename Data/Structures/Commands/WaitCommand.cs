using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class WaitCommand : NamedCommand
    {
        public override int Code => CommandCodes.Wait;
        public override string Name => "Wait";

        public int GetFrameCount()
        {
            return GetParameter<int>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return $"{GetFrameCount()} frame(s)";
        }
    }
}
