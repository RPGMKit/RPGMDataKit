using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
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

    public int ItemId { get; set; }
    public OperatorType Operator { get; set; }
    public OperandType Operand { get; set; }
    public int Value { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        ItemId = parameterProvider.GetParameter<int>(0);
        Operator = (OperatorType)parameterProvider.GetParameter<int>(1);
        Operand = (OperandType)parameterProvider.GetParameter<int>(2);
        Value = parameterProvider.GetParameter<int>(3);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, ItemId);
        parameterProvider.SetParameter(1, (int)Operator);
        parameterProvider.SetParameter(2, (int)Operand);
        parameterProvider.SetParameter(3, Value);
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