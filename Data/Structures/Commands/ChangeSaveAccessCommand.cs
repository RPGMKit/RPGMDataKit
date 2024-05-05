using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeSaveAccessCommand : NamedCommand
    {
        public override int Code => CommandCodes.ChangeSaveAccess;
        public override string GetCommandName(StorageVersion version) => "Change Save Access";

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
