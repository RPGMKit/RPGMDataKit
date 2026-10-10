using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeItemsCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeItems;
    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Change items";
        else
            return "Change Items";
    }
        
    public enum OperatorType
    {
        Add = 0,
        Take = 1,
    }

    public enum OperandType
    {
        Constant = 0,
        Variable = 1,
    }

    public int ItemId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public OperatorType Operator
    {
        get => Get<OperatorType>(1);
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

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatItem(ItemId);

        str += Operator switch
        {
            OperatorType.Add => " + ",
            OperatorType.Take => " - ",
            _ => throw new Exception($"Unknown change items operator '{Operator}'")
        };

        str += Operand switch
        {
            OperandType.Constant => Value,
            OperandType.Variable => context.NameFormatter.FormatVariable(Value),
            _ => throw new Exception($"Unknown change items operand '{Operand}'")
        };

        return str;
    }
}