using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.IfCommands;

public class IfWeaponCommand : IfCommand
{
    [EditorDefault(1)]
    public int WeaponId
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
        var weapon = context.NameFormatter.FormatWeapon(WeaponId);
        var str = context.Version == StorageVersion.MV ? $"Party has {weapon}" : $"{weapon} in Inventory";
        if (IncludeEquipment)
            str += context.Version == StorageVersion.MV ? " (Include Equipment)" : ", Include Equipment";

        return str;
    }
}