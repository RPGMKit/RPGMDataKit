using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class CallCommonEventCommand : NamedCommand
    {
        public override int Code => CommandCodes.CallCommonEvent;
        public override string GetCommandName(StorageVersion version)
        {
            return version == StorageVersion.VXAce ? "Call Common Event" : "Common Event";
        }

        public int GetEventId() => GetParameter<int>(0);

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return context.NameFormatter.FormatCommonEvent(GetEventId());
        }
    }
}
