using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

public class IfWeaponCommand : IfCommand
{
    public int GetWeapon()
    {
        return GetParameter<int>(1);
    }

    public bool GetIncludeEquipment()
    {
        return GetParameter<bool>(2);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var weapon = context.NameFormatter.FormatWeapon(GetWeapon());
        var str = context.Version == StorageVersion.MV ? $"Party has {weapon}" : $"{weapon} in Inventory";
        if (GetIncludeEquipment())
            str += context.Version == StorageVersion.MV ? " (Include Equipment)" : ", Include Equipment";

        return str;
    }
}