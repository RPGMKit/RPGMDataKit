using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class FadeoutScreenCommand : NamedCommand
    {
        public override int Code => CommandCodes.FadeoutScreen;
        public override string GetCommandName(StorageVersion version) => "Fadeout Screen";
    }
}
