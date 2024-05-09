using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeMenuAccessCommand : AbstractEnableAccessCommand
    {
        public override int Code => CommandCodes.ChangeMenuAccess;
        public override string GetCommandName(StorageVersion version) => "Change Menu Access";
    }
}
