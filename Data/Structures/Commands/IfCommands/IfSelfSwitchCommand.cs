using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfSelfSwitchCommand : IfCommand
    {
        public string GetSwitchName()
        {
            return GetParameter<string>(1);
        }

        public OnOffState GetState()
        {
            return (OnOffState)GetParameter<int>(2);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = $"Self Switch {GetSwitchName()}";

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
                    throw new Exception($"Unknown ON/OFF state for '{GetCommandName(context.Version)}' self switch command");
            }

            return str;
        }
    }
}
