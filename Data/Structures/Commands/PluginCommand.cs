using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class PluginCommand : NamedCommand
{
    public override int Code => CommandCodes.PluginCommand;
    public override string GetCommandName(StorageVersion version) => "Plugin Command";

    public string Command { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Command = parameterProvider.GetParameter<string>(0);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Command);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Command;
}
