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
            return GetParameter<int>(0);
        }

        public int GetOperator()
        {
            return GetParameter<int>(1);
        }

        public int GetOperandType()
        {
            return GetParameter<int>(2);
        }

        public int GetOperand()
        {
            return GetParameter<int>(3);
        }

        public bool GetIncludeEquipment()
        {
            return GetParameter<bool>(4);
        }

        public override int Code => CommandCodes.ChangeWeapons;
        public override string GetCommandName(StorageVersion version) => "Change Weapons";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatWeapon(GetWeapon());

            switch (GetOperator())
            {
                case OPERATOR_ADD:
                    str += " + ";
                    break;
                case OPERATOR_TAKE:
                    str += " - ";
                    break;
                default:
                    throw new Exception($"Unknown change weapons operator type '{GetOperator()}'");
            }

            switch (GetOperandType())
            {
                case OPERAND_CONSTANT:
                    str += GetOperand();
                    break;
                case OPERAND_VARIABLE:
                    str += context.NameFormatter.FormatVariable(GetOperand(),
                        new() { BracketType = context.Version == StorageVersion.MV ? BracketType.Figure : null });
                    break;
                default:
                    throw new Exception($"Unknown change weapons operand type '{GetOperandType()}'");
            }

            if (GetIncludeEquipment())
                str += context.Version == StorageVersion.MV ? " (Include Equipment)" : ", Include Equipment";

            return str;
        }
    }
}
