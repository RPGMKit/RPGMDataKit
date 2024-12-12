using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeWeaponsCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeWeapons;
    public override string GetCommandName(StorageVersion version) => "Change Weapons";
        
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

    public int WeaponId { get; set; }
    public OperatorType Operator { get; set; }
    public OperandType Operand { get; set; }
    public int Value { get; set; }
    public bool IncludeEquipment { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        WeaponId = parameterProvider.GetParameter<int>(0);
        Operator = (OperatorType)parameterProvider.GetParameter<int>(1);
        Operand = (OperandType)parameterProvider.GetParameter<int>(2);
        Value = parameterProvider.GetParameter<int>(3);
        IncludeEquipment = parameterProvider.GetParameter<bool>(4);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, WeaponId);
        parameterProvider.SetParameter(1, (int)Operator);
        parameterProvider.SetParameter(2, (int)Operand);
        parameterProvider.SetParameter(3, Value);
        parameterProvider.SetParameter(4, IncludeEquipment);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatWeapon(WeaponId);

        str += Operator switch
        {
            OperatorType.Add => " + ",
            OperatorType.Take => " - ",
            _ => throw new Exception($"Unknown change weapons operator '{Operator}'")
        };

        str += Operand switch
        {
            OperandType.Constant => Value,
            OperandType.Variable => context.NameFormatter.FormatVariable(Value),
            _ => throw new Exception($"Unknown change weapons operand type '{Operand}'")
        };

        if (IncludeEquipment)
            str += context.Version == StorageVersion.MV ? " (Include Equipment)" : ", Include Equipment";

        return str;
    }
}