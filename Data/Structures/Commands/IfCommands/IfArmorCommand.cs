using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.IfCommands;

public class IfArmorCommand : IfCommand
{
    [EditorDefault(1)]
    public int ArmorId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public bool IncludeEquipment
    {
        get => Get<bool>(2);
        set => Set(2, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var armor = context.NameFormatter.FormatArmor(ArmorId);
        var str = context.Version == StorageVersion.MV ? $"Party has {armor}" : $"{armor} in Inventory";
        if (IncludeEquipment)
            str += context.Version == StorageVersion.MV ? " (Include Equipment)" : ", Include Equipment";

        return str;
    }
}