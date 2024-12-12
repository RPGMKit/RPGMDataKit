using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChoiceCaseCommand : NamedCommand
{
    public ChoiceCaseCommand()
    {
        IsSecondary = true;
    }

    public override int Code => CommandCodes.ChoiceCase;
    public override string GetCommandName(StorageVersion version) => "When";

    public int ChoiceIndex { get; set; }
    public string ChoiceName { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        ChoiceIndex = parameterProvider.GetParameter<int>(0);
        ChoiceName = parameterProvider.GetParameter<string>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, ChoiceIndex);
        parameterProvider.SetParameter(1, ChoiceName);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.Version == StorageVersion.MV ? ChoiceName : $"[{ChoiceName}]";
    }
}