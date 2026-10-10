using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

public class IfWeaponCommand : IfCommand
{
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