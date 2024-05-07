using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

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

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = $"#{GetSwitch()}";

            var switchName = context.Storage.GetSwitch(GetSwitch());
            if (!string.IsNullOrEmpty(switchName))
                str += $" {switchName}";

            return str;
        }
    }
}