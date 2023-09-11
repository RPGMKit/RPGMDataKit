using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class EndIfCommand : NamedCommand
    {
        public EndIfCommand()
        {
            IsSecondary = true;
        }

        public override int Code => CommandCodes.EndIf;
        public override string Name => "End";
    }
}
