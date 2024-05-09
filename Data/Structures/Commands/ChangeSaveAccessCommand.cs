using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeSaveAccessCommand : AbstractEnableAccessCommand
    {
        public override int Code => CommandCodes.ChangeSaveAccess;
        public override string GetCommandName(StorageVersion version) => "Change Save Access";
    }
}
