using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class PluginCommand : NamedCommand
{
    public override int Code => CommandCodes.PluginCommand;
    public override string GetCommandName(StorageVersion version) => "Plugin Command";

    public string Command
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Command;
}
