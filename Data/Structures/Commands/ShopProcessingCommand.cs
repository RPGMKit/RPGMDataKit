using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShopProcessingCommand : NamedCommand
    {
        public enum ItemType
        {
            Item = 0,
            Weapon = 1,
            Armor = 2,
        }

        public ItemType GetItemType()
        {
            return (ItemType)GetParameter<int>(0);
        }

        public int GetId()
        {
            return GetParameter<int>(1);
        }

        public bool HasCustomPrice()
        {
            return GetParameter<int>(2) != 0;
        }

        public int GetCustomPrice()
        {
            return GetParameter<int>(3);
        }

        public bool GetPurchaseOnly()
        {
            return GetParameter<bool>(4);
        }

        public override int Code => CommandCodes.ShopProcessing;
        public override string GetCommandName(StorageVersion version) => "Shop Processing";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = GetItemType() switch
            {
                ItemType.Item => context.NameFormatter.FormatItem(GetId()),
                ItemType.Weapon => context.NameFormatter.FormatWeapon(GetId()),
                ItemType.Armor => context.NameFormatter.FormatArmor(GetId()),
                _ => throw new Exception($"Unknown item type {GetItemType()} in shop processing command")
            };

            if (HasCustomPrice())
                str += " (" + GetCustomPrice() + ")";

            return str;
        }
    }
}