using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveOpacityCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.Opacity;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Opacity";
        else
            return "Change Opacity";
    }
    
    public int Opacity { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Opacity = parameterProvider.GetParameter<int>(0);
    }
    
    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Opacity);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Opacity.ToString();
}