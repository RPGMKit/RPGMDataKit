using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class FadeinScreenCommand : NamedCommand
    {
        public override int Code => CommandCodes.FadeinScreen;
        public override string Name => "Fadein Screen";
    }
}
