using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

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

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return GetIsOn() ? "Enable" : "Disable";
        }
    }
}
