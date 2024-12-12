using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChoiceCaseCancelCommand : NamedCommand
{
    public ChoiceCaseCancelCommand()
    {
        IsSecondary = true;
    }

    public override int Code => CommandCodes.ChoiceCaseCancel;
    public override string GetCommandName(StorageVersion version) => "When";

    public object Unk0 { get; set; }
    public object Unk1 { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Unk0 = parameterProvider.GetParameter<object>(0);
        Unk1 = parameterProvider.GetParameter<object>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Unk0);
        parameterProvider.SetParameter(1, Unk1);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return "Cancel";
    }
}