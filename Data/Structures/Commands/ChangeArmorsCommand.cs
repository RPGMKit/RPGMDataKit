using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeArmorsCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeArmors;

    public override string GetCommandName(StorageVersion version)
    {
        return version == StorageVersion.MV ? "Change Armors" : "Change Armor";
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
        
    public int ArmorId { get; set; }
    public OperatorType Operator { get; set; }
    public OperandType Operand { get; set; }
    public int Value { get; set; }
    public bool IncludeEquipment { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        ArmorId = parameterProvider.GetParameter<int>(0);
        Operator = parameterProvider.GetParameter<OperatorType>(1);
        Operand = parameterProvider.GetParameter<OperandType>(2);
        Value = parameterProvider.GetParameter<int>(3);
        IncludeEquipment = parameterProvider.GetParameter<bool>(4);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, ArmorId);
        parameterProvider.SetParameter(1, Operator);
        parameterProvider.SetParameter(2, Operand);
        parameterProvider.SetParameter(3, Value);
        parameterProvider.SetParameter(4, IncludeEquipment);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatArmor(ArmorId);

        str += Operator switch
        {
            OperatorType.Add => " + ",
            OperatorType.Take => " - ",
            _ => throw new Exception($"Unknown change armors operator type '{Operator}'")
        };

        str += Operand switch
        {
            OperandType.Constant => Value,
            OperandType.Variable => context.NameFormatter.FormatVariable(Value),
            _ => throw new Exception($"Unknown change armors operand type '{Operand}'")
        };

        if (IncludeEquipment)
            str += context.Version == StorageVersion.MV ? " (Include Equipment)" : ", Include Equipment";

        return str;
    }
}