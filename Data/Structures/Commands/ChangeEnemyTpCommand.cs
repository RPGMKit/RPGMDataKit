using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

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

    public int TroopId
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