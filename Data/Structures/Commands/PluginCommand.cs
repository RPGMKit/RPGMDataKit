using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class PluginCommand : NamedCommand
    {
        public override int Code => CommandCodes.PluginCommand;
        public override string GetCommandName(StorageVersion version) => "Plugin Command";

        public string GetCommand()
        {
            return GetParameter<string>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return GetCommand();
        }
    }
}
