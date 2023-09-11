using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ReturnToTitleCommand : NamedCommand
    {
        public override int Code => CommandCodes.ReturnToTitle;
        public override string Name => "Return to Title Screen";
    }
}
