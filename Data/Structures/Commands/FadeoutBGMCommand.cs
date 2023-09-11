using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class FadeoutBGMCommand : NamedCommand
    {
        public override int Code => CommandCodes.FadeoutBgm;
        public override string Name => "Fadeout BGM";

        public int GetSeconds()
        {
            return (int)GetParameter(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return $"{GetSeconds()} second(s)";
        }
    }
}
