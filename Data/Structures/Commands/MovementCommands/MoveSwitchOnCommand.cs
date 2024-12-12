using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveSwitchOnCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.SwitchOn;
    public override string GetCommandName(StorageVersion version) => "Switch ON";

    public int SwitchId { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        SwitchId = parameterProvider.GetParameter<int>(0);
    }
    
    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, SwitchId);
    }
    
    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"{SwitchId:D4}";
    }
}