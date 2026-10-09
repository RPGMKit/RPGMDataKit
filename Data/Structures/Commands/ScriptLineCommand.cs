using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ScriptLineCommand : NamedCommand
{
    public ScriptLineCommand()
    {
        IsSecondary = true;
    }

    public override string GetCommandName(StorageVersion version) => "      ";
        
    public override int Code => CommandCodes.ScriptLine;

    public string Text { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Text = parameterProvider.GetParameter<string>(0);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Text);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Text;
}