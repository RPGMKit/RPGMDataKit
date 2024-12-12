using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class FadeoutBGSCommand : NamedCommand
{
    public override int Code => CommandCodes.FadeoutBgs;
    public override string GetCommandName(StorageVersion version) => "Fadeout BGS";

    public int Seconds { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Seconds = parameterProvider.GetParameter<int>(0);
    }
    
    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Seconds);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.Version == StorageVersion.MV ? $"{Seconds} second(s)" : $"{Seconds} sec.";
    }
}