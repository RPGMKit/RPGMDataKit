using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeTransparencyCommand : NamedCommand
    {
        protected const int SET_ON = 0;
        protected const int SET_OFF = 1;

        public override int Code => CommandCodes.ChangeTransparency;
        public override string GetCommandName(StorageVersion version) => "Change Transparency";

        public int GetOnOffState()
        {
            return GetParameter<int>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            switch (GetOnOffState())
            {
                case SET_ON:
                    return "ON";
                case SET_OFF:
                    return "OFF";
                default:
                    throw new Exception($"Unknown on/off state for change transparancy command");
            }
        }
    }
}