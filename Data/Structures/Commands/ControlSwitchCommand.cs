using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ControlSwitchCommand : NamedCommand
    {
        public int GetStartSwitch()
        {
            return GetParameter<int>(0);
        }

        public int GetEndSwitch()
        {
            return GetParameter<int>(1);
        }

        public OnOffState GetOnOffState()
        {
            return (OnOffState)GetParameter<int>(2);
        }

        public override int Code => CommandCodes.ControlSwitches;
        public override string GetCommandName(StorageVersion version) => "Control Switches";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            string str;
            if (GetStartSwitch() != GetEndSwitch())
                str = context.NameFormatter.FormatSwitchSpan(GetStartSwitch(), GetEndSwitch());
            else
            {
                if (context.Version == StorageVersion.MV)
                {
                    str = context.NameFormatter.FormatSwitch(GetStartSwitch(), new()
                    {
                        Separator = " ",
                        Prefix = "#",
                        SkipId = false,
                        SkipTitle = true,
                    });
                }
                else
                    str = context.NameFormatter.FormatSwitch(GetStartSwitch(), new() { SkipTitle = true });
            }

            switch (GetOnOffState())
            {
                case OnOffState.On:
                    str += " = ON";
                    break;
                case OnOffState.Off:
                    str += " = OFF";
                    break;
                default:
                    throw new Exception($"Unknown on/off state {GetOnOffState()} for action #{Code} {GetCommandName(context.Version)}");
            }

            return str;
        }
    }
}
