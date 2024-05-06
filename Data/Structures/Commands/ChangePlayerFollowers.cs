using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangePlayerFollowers : NamedCommand
    {
        protected const int SET_ON = 0;
        protected const int SET_OFF = 1;

        public override int Code => CommandCodes.ChangePlayerFollowers;
        public override string GetCommandName(StorageVersion version) => "Change Player Followers";

        public int GetOnOffState()
        {
            return GetParameter<int>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            switch (GetOnOffState())
            {
                case SET_ON:
                    return "ON";
                case SET_OFF:
                    return "OFF";
                default:
                    throw new Exception($"Unknown on/off state for change player followers command");
            }
        }
    }
}