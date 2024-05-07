using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

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

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return GetIsOn() ? "Enable" : "Disable";
        }
    }
}
