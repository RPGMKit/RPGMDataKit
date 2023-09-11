using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShopProcessingCommand : NamedCommand
    {
        protected const int TYPE_ITEM = 0;
        protected const int TYPE_WEAPON = 1;
        protected const int TYPE_ARMOR = 2;

        public int GetShopType()
        {
            return (int)GetParameter(0);
        }

        public int GetId()
        {
            return (int)GetParameter(1);
        }

        public bool HasCustomPrice()
        {
            return (int)GetParameter(2) != 0;
        }

        public int GetCustomPrice()
        {
            return (int)GetParameter(3);
        }

        public bool GetPurchaseOnly()
        {
            return (bool)GetParameter(4);
        }

        public override int Code => CommandCodes.ShopProcessing;
        public override string Name => "Shop Processing";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return $"#{GetId()}";
        }
    }
}