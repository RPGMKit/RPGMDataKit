using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfSwitchCommand : IfCommand
    {
        protected const int STATE_ON = 0;
        protected const int STATE_OFF = 1;

        public int GetSwitch()
        {
            return GetParameter<int>(1);
        }

        public int GetOnOff()
        {
            return GetParameter<int>(2);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatSwitch(GetSwitch());

            if (context.Version == StorageVersion.MV)
                str += " is ";
            else
                str += " == ";

            switch (GetOnOff())
            {
                case STATE_ON:
                    str += "ON";
                    break;
                case STATE_OFF:
                    str += "OFF";
                    break;
                default:
                    throw new Exception($"Unknown ON/OFF state for '{GetCommandName(context.Version)}' switch command");
            }

            return str;
        }
    }
}
