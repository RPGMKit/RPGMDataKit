using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveWaitCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.Wait;
    public override string GetCommandName(StorageVersion version) => "Wait";

    public int Frames { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Frames = parameterProvider.GetParameter<int>(0);
    }
    
    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Frames);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"{Frames} frame(s)";
    }
}