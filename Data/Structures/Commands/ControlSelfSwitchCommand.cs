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
            return (string)GetParameter(0);
        }

        public int GetOnOffState()
        {
            return (int)GetParameter(1);
        }

        public override int Code => CommandCodes.ControlSelfSwitch;
        public override string Name => "Control Self Switch";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
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
                    throw new Exception($"Unknown on/off state {GetOnOffState()} for action #{Code} {Name}");
            }

            return str;
        }
    }
}
