using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeWeaponsCommand : NamedCommand
    {
        protected const int OPERATOR_ADD = 0;
        protected const int OPERATOR_TAKE = 1;
        protected const int OPERAND_CONSTANT = 0;
        protected const int OPERAND_VARIABLE = 1;

        public int GetWeapon()
        {
            return (int)GetParameter(0);
        }

        public int GetOperator()
        {
            return (int)GetParameter(1);
        }

        public int GetOperandType()
        {
            return (int)GetParameter(2);
        }

        public int GetOperand()
        {
            return (int)GetParameter(3);
        }

        public bool GetIncludeEquipment()
        {
            return (bool)GetParameter(4);
        }

        public override int Code => CommandCodes.ChangeWeapons;
        public override string Name => "Change weapons";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"#{GetWeapon()} ";

            switch (GetOperator())
            {
                case OPERATOR_ADD:
                    str += "+";
                    break;
                case OPERATOR_TAKE:
                    str += "-";
                    break;
                default:
                    throw new Exception($"Unknown change weapons operator type '{GetOperator()}'");
            }

            str += " ";

            switch (GetOperandType())
            {
                case OPERAND_CONSTANT:
                    str += GetOperand();
                    break;
                case OPERAND_VARIABLE:
                    str += "{#" + GetOperand();
                    var variableName = storage.GetVariable(GetOperand());
                    if (!string.IsNullOrEmpty(variableName))
                        str += $" {variableName}";

                    str += "}";
                    break;
                default:
                    throw new Exception($"Unknown change weapons operand type '{GetOperandType()}'");
            }

            if (GetIncludeEquipment())
                str += " (Include Equipment)";

            return str;
        }
    }
}