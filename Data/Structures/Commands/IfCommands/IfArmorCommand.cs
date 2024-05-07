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
            var str = $"Party has armor #{GetArmor()}";
            if (GetIncludeEquipment())
                str += " (include Equipment)";

            return str;
        }
    }
}
