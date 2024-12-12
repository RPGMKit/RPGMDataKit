using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ScriptCommand : NamedCommand
{
    public override int Code => CommandCodes.Script;
    public override string GetCommandName(StorageVersion version) => "Script";

    public string Script { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Script = parameterProvider.GetParameter<string>(0);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Script);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Script;
}
