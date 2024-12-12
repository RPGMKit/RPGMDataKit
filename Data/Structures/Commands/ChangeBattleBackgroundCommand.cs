using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeBattleBackground : NamedCommand
{
    public override int Code => CommandCodes.ChangeBattleBackground;
    public override string GetCommandName(StorageVersion version) => "Change Battle Back";

    public string Image1 { get; set; }
    public string Image2 { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Image1 = parameterProvider.GetParameter<string>(0);
        Image2 = parameterProvider.GetParameter<string>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Image1);
        parameterProvider.SetParameter(1, Image2);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return !string.IsNullOrEmpty(Image2) ? $"{Image1} & {Image2}" : Image1;
    }
}