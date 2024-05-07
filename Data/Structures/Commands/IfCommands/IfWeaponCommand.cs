using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfWeaponCommand : IfCommand
    {
        public int GetWeapon()
        {
            return GetParameter<int>(1);
        }

        public bool GetIncludeEquipment()
        {
            return GetParameter<bool>(2);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = $"Party has weapon #{GetWeapon()}";
            if (GetIncludeEquipment())
                str += " (include Equipment)";

            return str;
        }
    }
}
