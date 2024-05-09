using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class FadeoutBGSCommand : NamedCommand
    {
        public override int Code => CommandCodes.FadeoutBgs;
        public override string GetCommandName(StorageVersion version) => "Fadeout BGS";

        public int GetSeconds()
        {
            return GetParameter<int>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return context.Version == StorageVersion.MV ? $"{GetSeconds()} second(s)" : $"{GetSeconds()} sec.";
        }
    }
}
