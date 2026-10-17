using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

/// <summary>The first item of a shop (the command 302); the items after it are <see cref="ShopProcessingLineCommand"/>.</summary>
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

    /// <summary>The shop buys nothing.</summary>
    public bool PurchaseOnly
    {
        get => Get<bool>(4);
        set => Set(4, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) =>
        Describe(context, Type, ItemId, HasCustomPrice);

    /// <summary>The text of an item of a shop, the same for the first one and for the others.</summary>
    internal static string Describe(EntityContext context, ItemType type, int itemId, bool hasCustomPrice)
    {
        var str = type switch
        {
            ItemType.Item => context.NameFormatter.FormatItem(itemId),
            ItemType.Weapon => context.NameFormatter.FormatWeapon(itemId),
            ItemType.Armor => context.NameFormatter.FormatArmor(itemId),
            _ => throw new Exception($"Unknown item type {type} in shop processing command")
        };

        if (hasCustomPrice)
            str += $" (CustomPrice)";

        return str;
    }
}
