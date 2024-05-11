using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class SelectItemCommand : NamedCommand
    {
        public enum KeyItemType
        {
            Regular = 1,
            Key = 2,
            HiddenA = 3,
            HiddenB = 4,
        }

        private static readonly Dictionary<KeyItemType, string> KeyItemTypeNames = new()
        {
            [KeyItemType.Regular] = "Regular Item",
            [KeyItemType.Key] = "Key Item",
            [KeyItemType.HiddenA] = "Hidden Item A",
            [KeyItemType.HiddenB] = "Hidden Item B",
        };

        public int GetVariable()
        {
            return GetParameter<int>(0);
        }

        public KeyItemType GetItemType()
        {
            return (KeyItemType)GetParameter<int>(1);
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
            var str = context.NameFormatter.FormatVariable(GetVariable(), new()
            {
                BracketType = context.Version == StorageVersion.MV ? BracketType.None : null
            });

            if (context.Version == StorageVersion.MV)
            {
                if (!KeyItemTypeNames.TryGetValue(GetItemType(), out var itemType))
                    throw new Exception($"Unknown item type for select item command: {GetItemType()}");

                str += $", {itemType}";
            }

            return str;
        }
    }
}
