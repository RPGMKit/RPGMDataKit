using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

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
    
    public ItemType Type { get; set; }
    public int ItemId { get; set; }
    public bool HasCustomPrice { get; set; }
    public int CustomPrice { get; set; }
    public bool PurchaseOnly { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Type = parameterProvider.GetParameter<ItemType>(0);
        ItemId = parameterProvider.GetParameter<int>(1);
        HasCustomPrice = parameterProvider.GetParameter<int>(2) != 0;
        CustomPrice = parameterProvider.GetParameter<int>(3);
        PurchaseOnly = parameterProvider.GetParameter<bool>(4);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Type);
        parameterProvider.SetParameter(1, ItemId);
        parameterProvider.SetParameter(2, HasCustomPrice ? 1 : 0);
        parameterProvider.SetParameter(3, CustomPrice);
        parameterProvider.SetParameter(4, PurchaseOnly);
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