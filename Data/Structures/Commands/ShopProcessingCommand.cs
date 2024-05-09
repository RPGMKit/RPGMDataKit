using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShopProcessingCommand : NamedCommand
    {
        protected const int TYPE_ITEM = 0;
        protected const int TYPE_WEAPON = 1;
        protected const int TYPE_ARMOR = 2;

        public int GetItemType()
        {
            return GetParameter<int>(0);
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
                TYPE_ITEM => context.NameFormatter.FormatItem(GetId()),
                TYPE_WEAPON => context.NameFormatter.FormatWeapon(GetId()),
                TYPE_ARMOR => context.NameFormatter.FormatArmor(GetId()),
                _ => throw new Exception($"Unknown item type {GetItemType()} in shop processing command")
            };

            if (HasCustomPrice())
                str += " (" + GetCustomPrice() + ")";

            return str;
        }
    }
}