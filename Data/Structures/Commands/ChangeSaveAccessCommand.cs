using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeSaveAccessCommand : NamedCommand
    {
        public override int Code => CommandCodes.ChangeSaveAccess;
        public override string Name => "Change Save Access";

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
