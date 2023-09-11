namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfArmorCommand : IfCommand
    {
        public int GetArmor()
        {
            return (int)GetParameter(1);
        }

        public bool GetIncludeEquipment()
        {
            return (bool)GetParameter(2);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"Party has armor #{GetArmor()}";
            if (GetIncludeEquipment())
                str += " (include Equipment)";

            return str;
        }
    }
}
