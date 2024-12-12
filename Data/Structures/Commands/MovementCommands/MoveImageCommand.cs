using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveImageCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.Image;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Image";
        else
            return "Graphic";
    }
    
    public string Image { get; set; }
    public int Offset { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Image = parameterProvider.GetParameter<string>(0);
        Offset = parameterProvider.GetParameter<int>(1);
    }
    
    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Image);
        parameterProvider.SetParameter(1, Offset);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        if (string.IsNullOrWhiteSpace(Image))
            return context.Version == StorageVersion.MV ? "None" : "(None)";

        if (context.Version == StorageVersion.MV)
            return $"{Image}({Offset})";
        else
            return $"'{Image}', {Offset}";
    }
}