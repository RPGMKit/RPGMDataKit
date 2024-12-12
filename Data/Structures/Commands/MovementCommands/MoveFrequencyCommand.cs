using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveFrequencyCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.Frequency;
    public override string GetCommandName(StorageVersion version) => "Frequency";

    public int Frequency { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Frequency = parameterProvider.GetParameter<int>(0);
    }
    
    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Frequency);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Frequency.ToString();
}
