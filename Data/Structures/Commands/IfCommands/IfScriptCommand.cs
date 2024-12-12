using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

public class IfScriptCommand : IfCommand
{
    public string Script { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Unpack(parameterProvider, version);

        Script = parameterProvider.GetParameter<string>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Pack(parameterProvider, version);
        
        parameterProvider.SetParameter(1, Script);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"Script: {Script}";
    }
}