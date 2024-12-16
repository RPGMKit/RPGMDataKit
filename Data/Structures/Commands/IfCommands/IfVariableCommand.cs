using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

public class IfVariableCommand : IfCommand
{
    public enum OperatorType
    {
        Eq = 0,
        Gte = 1,
        Lte = 2,
        Gt = 3,
        Lt = 4,
        NEq = 5,
    }

    public enum OperandType
    {
        Constant = 0,
        Variable = 1,
    }
    
    public int VariableId { get; set; }
    public OperandType Operand { get; set; }
    public int Value { get; set; }
    public OperatorType Operator { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Unpack(parameterProvider, version);

        VariableId = parameterProvider.GetParameter<int>(1);
        Operand = parameterProvider.GetParameter<OperandType>(2);
        Value = parameterProvider.GetParameter<int>(3);
        Operator = parameterProvider.GetParameter<OperatorType>(4);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Pack(parameterProvider, version);
        
        parameterProvider.SetParameter(1, VariableId);
        parameterProvider.SetParameter(2, Operand);
        parameterProvider.SetParameter(3, Value);
        parameterProvider.SetParameter(4, Operator);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatVariable(VariableId, new()
        {
            BracketType = context.Version == StorageVersion.MV ? BracketType.None : null
        });

        str += Operator switch
        {
            OperatorType.Eq => context.Version == StorageVersion.MV ? " = " : " == ",
            OperatorType.Gte => context.Version == StorageVersion.MV ? " ≥ " : " >= ",
            OperatorType.Lte => context.Version == StorageVersion.MV ? " ≤ " : " <= ",
            OperatorType.Gt => " > ",
            OperatorType.Lt => " < ",
            OperatorType.NEq => context.Version == StorageVersion.MV ? " ≠ " : " != ",
            _ => throw new Exception(
                $"Unknown operator {Operator} for '{GetCommandName(context.Version)}' variable command")
        };

        switch (Operand)
        {
            case OperandType.Constant:
                str += Value;
                break;
            case OperandType.Variable:
                str += context.NameFormatter.FormatVariable(Value, new()
                {
                    BracketType = context.Version == StorageVersion.MV ? BracketType.None : null
                });
                break;
            default:
                throw new Exception(
                    $"Unknown operand '{Operand}' for '{GetCommandName(context.Version)}' variable command");
        }

        return str;
    }
}