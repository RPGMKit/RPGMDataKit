using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class EndLoopCommand : NamedCommand
    {
        public EndLoopCommand()
        {
            IsSecondary = true;
        }

        public override int Code => CommandCodes.EndLoop;
        public override string GetCommandName(StorageVersion version) => "Repeat Above";
    }
}
