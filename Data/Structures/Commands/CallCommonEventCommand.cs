using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class CallCommonEventCommand : NamedCommand
    {
        public override int Code => CommandCodes.CallCommonEvent;
        public override string GetCommandName(StorageVersion version)
        {
            return version == StorageVersion.VXAce ? "Call Common Event" : "Common Event";
        }

        public int GetEventId()
        {
            return GetParameter<int>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var cCommonEvent = storage.GetCommonEvent(GetEventId());

            return $"#{GetEventId()}" + (cCommonEvent != null ? " " + cCommonEvent.Name : "");
        }
    }
}
