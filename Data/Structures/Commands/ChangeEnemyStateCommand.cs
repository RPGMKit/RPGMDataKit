using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeEnemyStateCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeEnemyState;
    public override string GetCommandName(StorageVersion version) => "Change Enemy State";
        
    public enum OperatorType
    {
        Add = 0,
        Remove = 1,
    }

    public int TroopId { get; set; }
    public OperatorType Operator { get; set; }
    public int StateId { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        TroopId = parameterProvider.GetParameter<int>(0);
        Operator = parameterProvider.GetParameter<OperatorType>(1);
        StateId = parameterProvider.GetParameter<int>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, TroopId);
        parameterProvider.SetParameter(1, Operator);
        parameterProvider.SetParameter(2, StateId);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatTroopMember(TroopId);

        str += Operator switch
        {
            OperatorType.Add => ", + ",
            OperatorType.Remove => ", - ",
            _ => throw new Exception($"Unknown change enemy state operator '{Operator}'")
        };

        str += context.NameFormatter.FormatState(StateId);

        return str;
    }
}