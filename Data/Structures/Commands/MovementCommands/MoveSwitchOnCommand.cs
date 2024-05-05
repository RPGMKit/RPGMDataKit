using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveSwitchOnCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.SwitchOn;
        public override string GetCommandName(StorageVersion version) => "Switch ON";

        public int GetSwitch()
        {
            return GetParameter<int>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"#{GetSwitch()}";

            var switchName = storage.GetSwitch(GetSwitch());
            if (!string.IsNullOrEmpty(switchName))
                str += $" {switchName}";

            return str;
        }
    }
}