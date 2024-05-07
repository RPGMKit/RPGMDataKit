using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ControlSelfSwitchCommand : NamedCommand
    {
        protected const int ON_STATE = 0;
        protected const int OFF_STATE = 1;

        public string GetSwitch()
        {
            return GetParameter<string>(0);
        }

        public int GetOnOffState()
        {
            return GetParameter<int>(1);
        }

        public override int Code => CommandCodes.ControlSelfSwitch;
        public override string GetCommandName(StorageVersion version) => "Control Self Switch";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = GetSwitch();
            switch (GetOnOffState())
            {
                case ON_STATE:
                    str += " = ON";
                    break;
                case OFF_STATE:
                    str += " = OFF";
                    break;
                default:
                    throw new Exception($"Unknown on/off state {GetOnOffState()} for action #{Code} {nameof(ControlSelfSwitchCommand)}");
            }

            return str;
        }
    }
}
