using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

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
        
    public SourceType SourceT { get; set; }
    public int SourceId { get; set; }
    public ActionParameterType ActionParameter { get; set; }
    public OperatorType Operator { get; set; }
    public OperandType Operand { get; set; }
    public int Value { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        SourceT = (SourceType)parameterProvider.GetParameter<int>(0);
        SourceId = parameterProvider.GetParameter<int>(1);
        ActionParameter = (ActionParameterType)parameterProvider.GetParameter<int>(2);
        Operator = (OperatorType)parameterProvider.GetParameter<int>(3);
        Operand = (OperandType)parameterProvider.GetParameter<int>(4);
        Value = parameterProvider.GetParameter<int>(5);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, (int)SourceT);
        parameterProvider.SetParameter(1, SourceId);
        parameterProvider.SetParameter(2, (int)ActionParameter);
        parameterProvider.SetParameter(3, (int)Operator);
        parameterProvider.SetParameter(4, (int)Operand);
        parameterProvider.SetParameter(5, Value);
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