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

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"#{GetSwitch()}";
            var switchName = storage.GetSwitch(GetSwitch());
            if (!string.IsNullOrEmpty(switchName))
                str += $" {switchName}";

            str += " is ";
            switch (GetOnOff())
            {
                case STATE_ON:
                    str += "ON";
                    break;
                case STATE_OFF:
                    str += "OFF";
                    break;
                default:
                    throw new Exception($"Unknown ON/OFF state for '{Name}' switch command");
            }

            return str;
        }
    }
}
