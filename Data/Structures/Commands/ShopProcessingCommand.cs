using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ShopProcessingCommand : NamedCommand
{
    public override int Code => CommandCodes.ShopProcessing;
    public override string GetCommandName(StorageVersion version) => "Shop Processing";
    
    public enum ItemType
    {
        Item = 0,
        Weapon = 1,
        Armor = 2,
    }
    
    public ItemType Type
    {
        get => Get<ItemType>(0);
        set => Set(0, value);
    }

    public int ItemId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public bool HasCustomPrice
    {
        get => Get<int>(2) != 0;
        set => Set(2, value ? 1 : 0);
    }

    public int CustomPrice
    {
        get => Get<int>(3);
        set => Set(3, value);
    }

    public bool PurchaseOnly
    {
        get => Get<bool>(4);
        set => Set(4, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = Type switch
        {
            ItemType.Item => context.NameFormatter.FormatItem(ItemId),
            ItemType.Weapon => context.NameFormatter.FormatWeapon(ItemId),
            ItemType.Armor => context.NameFormatter.FormatArmor(ItemId),
            _ => throw new Exception($"Unknown item type {Type} in shop processing command")
        };

        if (HasCustomPrice)
            str += $" (CustomPrice)";

        return str;
    }
}