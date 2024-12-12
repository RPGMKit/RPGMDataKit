using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ErasePictureCommand : NamedCommand
{
    public override int Code => CommandCodes.ErasePicture;
    public override string GetCommandName(StorageVersion version) => "Erase Picture";

    public int Layer { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Layer = parameterProvider.GetParameter<int>(0);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Layer);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.Version == StorageVersion.MV ? $"#{Layer}" : Layer.ToString();
    }
}