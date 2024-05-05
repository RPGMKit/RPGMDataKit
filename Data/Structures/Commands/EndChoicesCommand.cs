using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class EndChoicesCommand : NamedCommand
    {
        public EndChoicesCommand()
        {
            IsSecondary = true;
        }

        public override int Code => CommandCodes.ChoicesEnd;
        public override string GetCommandName(StorageVersion version) => "End";
    }
}
