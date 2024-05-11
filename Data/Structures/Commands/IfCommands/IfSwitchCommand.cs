using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfSwitchCommand : IfCommand
    {
        public int GetSwitch()
        {
            return GetParameter<int>(1);
        }

        public OnOffState GetState()
        {
            return (OnOffState)GetParameter<int>(2);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatSwitch(GetSwitch());

            if (context.Version == StorageVersion.MV)
                str += " is ";
            else
                str += " == ";

            switch (GetState())
            {
                case OnOffState.On:
                    str += "ON";
                    break;
                case OnOffState.Off:
                    str += "OFF";
                    break;
                default:
                    throw new Exception($"Unknown ON/OFF state for '{GetCommandName(context.Version)}' switch command");
            }

            return str;
        }
    }
}
