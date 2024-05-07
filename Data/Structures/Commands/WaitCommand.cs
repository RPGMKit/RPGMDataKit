using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class WaitCommand : NamedCommand
    {
        public override int Code => CommandCodes.Wait;
        public override string GetCommandName(StorageVersion version) => "Wait";

        public int GetFrameCount()
        {
            return GetParameter<int>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return $"{GetFrameCount()} frame(s)";
        }
    }
}
