using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

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
        
    public int ArmorId
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

    /// <summary>Counts the armor that is equipped too. It is used only when the armor is taken: with <c>Add</c> it is always false (the editors turn it off).</summary>
    public bool IncludeEquipment
    {
        get => Operator == OperatorType.Take && Get<bool>(4);
        set => Set(4, value);
    }

    // a new command is for the armor and the count 1 in the editors
    protected override void OnBinding(StorageVersion version)
    {
        if (!IsParameterSet(0))
            Set(0, 1);
        if (!IsParameterSet(3))
            Set(3, 1);
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