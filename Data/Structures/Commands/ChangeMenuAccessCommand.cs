using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeMenuAccessCommand : NamedCommand
    {
        public override int Code => CommandCodes.ChangeMenuAccess;
        public override string GetCommandName(StorageVersion version) => "Change Menu Access";

        public bool GetIsOn()
        {
            return GetParameter<int>(0) == 1;
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return GetIsOn() ? "Enable" : "Disable";
        }
    }
}
