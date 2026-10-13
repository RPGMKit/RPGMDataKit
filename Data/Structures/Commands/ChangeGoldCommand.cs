using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

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

    public OperatorType Operator
    {
        get => Get<OperatorType>(0);
        set => Set(0, value);
    }

    public OperandType Operand
    {
        get => Get<OperandType>(1);
        set => Set(1, value);
    }

    public int Value
    {
        get => Get<int>(2);
        set => Set(2, value);
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