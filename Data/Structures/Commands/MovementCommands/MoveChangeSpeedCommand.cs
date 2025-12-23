using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveChangeSpeedCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.ChangeSpeed;
    public override string GetCommandName(StorageVersion version) => "Speed";

    public int Speed { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Speed = parameterProvider.GetParameter<int>(0);
    }
    
    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Speed);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Speed.ToString();
}