using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class SelectItemCommand : NamedCommand
    {
        public int GetVariable()
        {
            return (int)GetParameter(0);
        }

        public int GetItemType()
        {
            return (int)GetParameter(1);
        }

        public override int Code => CommandCodes.SelectItem;
        public override string Name => "Select item";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"#{GetVariable()}";
            var variableName = storage.GetVariable(GetVariable());
            if (!string.IsNullOrEmpty(variableName))
                str += $" {variableName}";

            str += $", item type #{GetItemType()}";

            return str;
        }
    }
}
