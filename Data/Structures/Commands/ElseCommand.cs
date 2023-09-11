using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ElseCommand : NamedCommand
    {
        public ElseCommand()
        {
            IsSecondary = true;
        }

        public override int Code => CommandCodes.Else;
        public override string Name => "Else";
    }
}
