using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class SelectItemCommand : NamedCommand
    {
        public int GetVariable()
        {
            return GetParameter<int>(0);
        }

        public int GetItemType()
        {
            return GetParameter<int>(1);
        }

        public override int Code => CommandCodes.SelectItem;
        public override string GetCommandName(StorageVersion version) => "Select item";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = $"#{GetVariable()}";
            var variableName = context.Storage.GetVariable(GetVariable());
            if (!string.IsNullOrEmpty(variableName))
                str += $" {variableName}";

            str += $", item type #{GetItemType()}";

            return str;
        }
    }
}
