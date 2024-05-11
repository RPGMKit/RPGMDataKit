using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ControlSelfSwitchCommand : NamedCommand
    {
        public string GetSwitch()
        {
            return GetParameter<string>(0);
        }

        public OnOffState GetOnOffState()
        {
            return (OnOffState)GetParameter<int>(1);
        }

        public override int Code => CommandCodes.ControlSelfSwitch;
        public override string GetCommandName(StorageVersion version) => "Control Self Switch";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = GetSwitch();
            switch (GetOnOffState())
            {
                case OnOffState.On:
                    str += " = ON";
                    break;
                case OnOffState.Off:
                    str += " = OFF";
                    break;
                default:
                    throw new Exception($"Unknown on/off state {GetOnOffState()} for action #{Code} {nameof(ControlSelfSwitchCommand)}");
            }

            return str;
        }
    }
}
