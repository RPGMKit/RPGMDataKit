using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

/// <summary>
/// An item of a shop after the first one (the command 605). It has the type, the item, the flag of the price and the price, in both makers: the
/// parameter "purchase only" belongs to the first item, <see cref="ShopProcessingCommand"/>.
/// </summary>
public class ShopProcessingLineCommand : NamedCommand
{
    public ShopProcessingLineCommand()
    {
        IsSecondary = true;
    }

    public override int Code => CommandCodes.ShopProcessingLine;
    public override string GetCommandName(StorageVersion version) => "               ";

    public ShopProcessingCommand.ItemType Type
    {
        get => Get<ShopProcessingCommand.ItemType>(0);
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

    public override string GetParametersDescription(EntityContext context, DumpOptions options) =>
        ShopProcessingCommand.Describe(context, Type, ItemId, HasCustomPrice);
}
