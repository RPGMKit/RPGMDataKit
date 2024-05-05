using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ControlSwitchCommand : NamedCommand
    {
        protected const int ON_STATE = 0;
        protected const int OFF_STATE = 1;

        public int GetStartSwitch()
        {
            return GetParameter<int>(0);
        }

        public int GetEndSwitch()
        {
            return GetParameter<int>(1);
        }

        public int GetOnOffState()
        {
            return GetParameter<int>(2);
        }

        public override int Code => CommandCodes.ControlSwitches;
        public override string Name => "Control Switches";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = "";
            if (GetStartSwitch() != GetEndSwitch())
                str = $"#{GetStartSwitch()}..{GetEndSwitch()}";
            else
            {
                str = $"#{GetStartSwitch()}";
                var switchName = storage.GetSwitch(GetStartSwitch());
                if (!string.IsNullOrEmpty(switchName))
                {
                    str += $" {switchName}";
                }
            }

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
