using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeEnemyTpCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeEnemyTp;
    public override string GetCommandName(StorageVersion version) => "Change enemy TP";
        
    public enum OperatorType
    {
        Increase = 0,
        Decrease = 1,
    }

    public enum OperandType
    {
        Constant = 0,
        Variable = 1,
    }

    public int TroopId { get; set; }
    public OperatorType Operator { get; set; }
    public OperandType Operand { get; set; }
    public int Value { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        TroopId = parameterProvider.GetParameter<int>(0);
        Operator = parameterProvider.GetParameter<OperatorType>(1);
        Operand = parameterProvider.GetParameter<OperandType>(2);
        Value = parameterProvider.GetParameter<int>(3);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, TroopId);
        parameterProvider.SetParameter(1, Operator);
        parameterProvider.SetParameter(2, Operand);
        parameterProvider.SetParameter(3, Value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatTroopMember(TroopId);

        str += Operator switch
        {
            OperatorType.Increase => ", + ",
            OperatorType.Decrease => ", - ",
            _ => throw new Exception($"Unknown modify enemy tp operator '{Operator}'")
        };

        str += Operand switch
        {
            OperandType.Constant => Value,
            OperandType.Variable => context.NameFormatter.FormatVariable(Value),
            _ => throw new Exception($"Unknown modify enemy tp operand '{Operand}'")
        };

        return str;
    }
}