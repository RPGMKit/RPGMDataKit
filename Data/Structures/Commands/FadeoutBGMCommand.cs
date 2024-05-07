using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class FadeoutBGMCommand : NamedCommand
    {
        public override int Code => CommandCodes.FadeoutBgm;
        public override string GetCommandName(StorageVersion version) => "Fadeout BGM";

        public int GetSeconds()
        {
            return GetParameter<int>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return $"{GetSeconds()} second(s)";
        }
    }
}
