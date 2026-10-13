using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeTpCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeTp;
    public override string GetCommandName(StorageVersion version) => "Change TP";
        
    public enum SourceType
    {
        Actor = 0,
        Variable = 1,
    }

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

    public SourceType SourceT
    {
        get => Get<SourceType>(0);
        set => Set(0, value);
    }

    public int SourceId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public OperatorType Operator
    {
        get => Get<OperatorType>(2);
        set => Set(2, value);
    }

    public OperandType Operand
    {
        get => Get<OperandType>(3);
        set => Set(3, value);
    }

    public int Value
    {
        get => Get<int>(4);
        set => Set(4, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = SourceT switch
        {
            SourceType.Actor => context.NameFormatter.FormatActor(SourceId),
            SourceType.Variable => context.NameFormatter.FormatVariable(SourceId),
            _ => throw new Exception($"Unknown modify tp source type '{SourceT}'")
        };

        str += Operator switch
        {
            OperatorType.Increase => ", + ",
            OperatorType.Decrease => ", - ",
            _ => throw new Exception($"Unknown modify tp operator '{Operator}'")
        };

        str += Operand switch
        {
            OperandType.Constant => Value,
            OperandType.Variable => context.NameFormatter.FormatVariable(Value),
            _ => throw new Exception($"Unknown modify tp operand '{Operand}'")
        };

        return str;
    }
}