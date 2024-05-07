using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

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
        public override string GetCommandName(StorageVersion version) => "Control Switches";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = "";
            if (GetStartSwitch() != GetEndSwitch())
                str = $"#{GetStartSwitch()}..{GetEndSwitch()}";
            else
            {
                str = $"#{GetStartSwitch()}";
                var switchName = context.Storage.GetSwitch(GetStartSwitch());
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
                    throw new Exception($"Unknown on/off state {GetOnOffState()} for action #{Code} {GetCommandName(context.Version)}");
            }

            return str;
        }
    }
}
