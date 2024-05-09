using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class SelectItemCommand : NamedCommand
    {
        private static Dictionary<int, string> KeyItemType = new()
        {
            [1] = "Regular Item",
            [2] = "Key Item",
            [3] = "Hidden Item A",
            [4] = "Hidden Item B",
        };

        public int GetVariable()
        {
            return GetParameter<int>(0);
        }

        public int GetItemType()
        {
            return GetParameter<int>(1);
        }

        public override int Code => CommandCodes.SelectItem;

        public override string GetCommandName(StorageVersion version)
        {
            if (version == StorageVersion.MV)
                return "Select Item";
            else
                return "Select Key Item";
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatVariable(GetVariable());
            if (context.Version == StorageVersion.MV)
            {
                if (!KeyItemType.TryGetValue(GetItemType(), out var itemType))
                    throw new Exception($"Unknown item type for select item command: {GetItemType()}");

                str += $", {itemType}";
            }

            return str;
        }
    }
}
