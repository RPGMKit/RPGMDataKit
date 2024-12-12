using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeGoldCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeGold;
    public override string GetCommandName(StorageVersion version) => "Change gold";
        
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

    public OperatorType Operator { get; set; }
    public OperandType Operand { get; set; }
    public int Value { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Operator = (OperatorType)parameterProvider.GetParameter<int>(0);
        Operand = (OperandType)parameterProvider.GetParameter<int>(1);
        Value = parameterProvider.GetParameter<int>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, (int)Operator);
        parameterProvider.SetParameter(1, (int)Operand);
        parameterProvider.SetParameter(2, Value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = Operator switch
        {
            OperatorType.Increase => "+ ",
            OperatorType.Decrease => "- ",
            _ => throw new Exception($"Unknown modify gold operator '{Operator}'")
        };

        str += Operand switch
        {
            OperandType.Constant => Value,
            OperandType.Variable => context.NameFormatter.FormatVariable(Value),
            _ => throw new Exception($"Unknown modify gold operand '{Operand}'")
        };

        return str;
    }
}