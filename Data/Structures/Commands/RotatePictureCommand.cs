using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class RotatePictureCommand : NamedCommand
{
    public override int Code => CommandCodes.RotatePicture;
    public override string GetCommandName(StorageVersion version) => "Rotate Picture";

    public int Layer { get; set; }
    public int Speed { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Layer = parameterProvider.GetParameter<int>(0);
        Speed = parameterProvider.GetParameter<int>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Layer);
        parameterProvider.SetParameter(1, Speed);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        if (context.Version == StorageVersion.MV)
            return $"#{Layer}, {Speed}";

        if (Speed >= 0)
            return $"{Layer}, +{Speed}";

        return $"{Layer}, {Speed}";
    }
}