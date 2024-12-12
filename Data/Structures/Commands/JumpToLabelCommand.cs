using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class JumpToLabelCommand : NamedCommand
{
    public override int Code => CommandCodes.JumpToLabel;
    public override string GetCommandName(StorageVersion version) => "Jump to Label";

    public string Label { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Label = parameterProvider.GetParameter<string>(0);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Label);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Label;
}