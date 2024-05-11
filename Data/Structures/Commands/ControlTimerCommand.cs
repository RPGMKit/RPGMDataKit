using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ControlTimerCommand : NamedCommand
    {
        public OnOffState GetState()
        {
            return (OnOffState)GetParameter<int>(0);
        }

        public int GetSeconds()
        {
            return GetParameter<int>(1);
        }

        public override int Code => CommandCodes.ControlTimer;
        public override string GetCommandName(StorageVersion version) => "Control Timer";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            if (GetState() == OnOffState.Off)
                return "Stop";

            var sec = GetSeconds();
            var min = sec / 60;
            sec %= 60;

            if (context.Version == StorageVersion.MV)
                return $"Start, {min} min {sec} sec";
            else
                return $"Startup ({min} min. {sec} sec.)";
        }
    }
}
