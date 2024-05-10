using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfArmorCommand : IfCommand
    {
        public int GetArmor()
        {
            return GetParameter<int>(1);
        }

        public bool GetIncludeEquipment()
        {
            return GetParameter<bool>(2);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var armor = context.NameFormatter.FormatArmor(GetArmor());
            var str = context.Version == StorageVersion.MV ? $"Party has {armor}" : $"{armor} in Inventory";
            if (GetIncludeEquipment())
                str += context.Version == StorageVersion.MV ? " (Include Equipment)" : ", Include Equipment";

            return str;
        }
    }
}
