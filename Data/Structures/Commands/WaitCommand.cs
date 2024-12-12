using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class WaitCommand : NamedCommand
{
    public override int Code => CommandCodes.Wait;
    public override string GetCommandName(StorageVersion version) => "Wait";

    public int FrameCount { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        FrameCount = parameterProvider.GetParameter<int>(0);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, FrameCount);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"{FrameCount} frame(s)";
    }
}