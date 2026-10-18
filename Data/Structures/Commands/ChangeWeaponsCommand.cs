using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

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

    [EditorDefault(1)]
    public int WeaponId
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

    [EditorDefault(1)]
    public int Value
    {
        get => Get<int>(3);
        set => Set(3, value);
    }

    /// <summary>Counts the weapon that is equipped too. It is used only when the weapon is taken: with <c>Add</c> it is always false (the editors turn it off).</summary>
    public bool IncludeEquipment
    {
        get => Operator == OperatorType.Take && Get<bool>(4);
        set => Set(4, value);
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