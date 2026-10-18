using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeParameterCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeParameter;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Change Parameter";
        else
            return "Change Parameters";
    }
        
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

    public enum ActionParameterType
    {
        MaxHp = 0,
        MaxMp = 1,
        Attack = 2,
        Defense = 3,
        MAttack = 4,
        MDefense = 5,
        Agility = 6,
        Luck = 7,
    }

    private static readonly Dictionary<ActionParameterType, Tuple<string,string>> ParameterNamesMap = new()
    {
        [ActionParameterType.MaxHp] = new("Max HP", "MHP"),
        [ActionParameterType.MaxMp] = new("Max MP", "MMP"),
        [ActionParameterType.Attack] = new("Attack","ATK"),
        [ActionParameterType.Defense] = new("Defense", "DEF"),
        [ActionParameterType.MAttack] = new("M.Attack","MAT"),
        [ActionParameterType.MDefense] = new("M.Defense", "MDF"),
        [ActionParameterType.Agility] = new("Agility", "AGI"),
        [ActionParameterType.Luck] = new("Luck", "LUK"),
    };
        
    public SourceType SourceT
    {
        get => Get<SourceType>(0);
        set => Set(0, value);
    }

    [EditorDefault(1, StorageVersion.VXAce)]   // the editor of MV opens with the entire party (0)
    public int SourceId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public ActionParameterType ActionParameter
    {
        get => Get<ActionParameterType>(2);
        set => Set(2, value);
    }

    public OperatorType Operator
    {
        get => Get<OperatorType>(3);
        set => Set(3, value);
    }

    public OperandType Operand
    {
        get => Get<OperandType>(4);
        set => Set(4, value);
    }

    [EditorDefault(1)]
    public int Value
    {
        get => Get<int>(5);
        set => Set(5, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = SourceT switch
        {
            SourceType.Actor => context.NameFormatter.FormatActor(SourceId),
            SourceType.Variable => context.NameFormatter.FormatVariable(SourceId),
            _ => throw new Exception($"Unknown change parameter source type '{SourceT}'")
        };

        if (!ParameterNamesMap.TryGetValue(ActionParameter, out var actionName))
            throw new Exception($"Unknown action parameter {ActionParameter} for change parameter command");
            
        str += ", " + (context.Version == StorageVersion.MV ? actionName.Item1 : actionName.Item2);

        str += Operator switch
        {
            OperatorType.Increase => " + ",
            OperatorType.Decrease => " - ",
            _ => throw new Exception($"Unknown change parameter operator '{Operator}'")
        };

        str += Operand switch
        {
            OperandType.Constant => Value,
            OperandType.Variable => context.NameFormatter.FormatVariable(Value),
            _ => throw new Exception($"Unknown change parameter operand '{Operand}'")
        };

        return str;
    }
}