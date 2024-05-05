using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ControlTimerCommand : NamedCommand
    {
        public bool GetOn()
        {
            return GetParameter<int>(0) == 0;
        }

        public int? GetSeconds()
        {
            return GetParameter<int>(1);
        }

        public override int Code => CommandCodes.ControlTimer;
        public override string Name => "Control Timer";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            if (!GetOn())
                return "Stop";

            var str = $"Start, " + SecondsToTimeStr(GetSeconds() ?? 0);
            
            return str;
        }

        private string SecondsToTimeStr(int value)
        {
            var min = value / 60;
            var sec = value % 60;

            return $"{min} min {sec} sec";
        }
    }
}
