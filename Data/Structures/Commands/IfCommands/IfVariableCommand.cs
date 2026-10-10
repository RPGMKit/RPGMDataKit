using RPGMakerLibCoreV2.Data.Enums;
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
    
    public int VariableId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public OperandType Operand
    {
        get => Get<OperandType>(2);
        set => Set(2, value);
    }

    public int Value
    {
        get => Get<int>(3);
        set => Set(3, value);
    }

    public OperatorType Operator
    {
        get => Get<OperatorType>(4);
        set => Set(4, value);
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