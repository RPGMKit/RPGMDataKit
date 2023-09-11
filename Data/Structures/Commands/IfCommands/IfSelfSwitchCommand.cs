namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfSelfSwitchCommand : IfCommand
    {
        protected const int STATE_ON = 0;
        protected const int STATE_OFF = 1;

        public string GetSwitchName()
        {
            return (string)GetParameter(1);
        }

        public int GetOnOff()
        {
            return (int)GetParameter(2);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"Self Switch {GetSwitchName()} is ";

            switch (GetOnOff())
            {
                case STATE_ON:
                    str += "ON";
                    break;
                case STATE_OFF:
                    str += "OFF";
                    break;
                default:
                    throw new Exception($"Unknown ON/OFF state for '{Name}' self switch command");
            }

            return str;
        }
    }
}
