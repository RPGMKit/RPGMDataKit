using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeArmorsCommand : NamedCommand
    {
        public enum OperatorType
        {
            Add = 0,
            Take = 1,
        }

        public enum OperandType
        {
            Constant = 0,
            Variable = 1,
        }

        public int GetArmor()
        {
            return GetParameter<int>(0);
        }

        public OperatorType GetOperator()
        {
            return (OperatorType)GetParameter<int>(1);
        }

        public OperandType GetOperandType()
        {
            return (OperandType)GetParameter<int>(2);
        }

        public int GetOperand()
        {
            return GetParameter<int>(3);
        }

        public bool GetIncludeEquipment()
        {
            return GetParameter<bool>(4);
        }

        public override int Code => CommandCodes.ChangeArmors;

        public override string GetCommandName(StorageVersion version)
        {
            return version == StorageVersion.MV ? "Change Armors" : "Change Armor";
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatArmor(GetArmor());

            switch (GetOperator())
            {
                case OperatorType.Add:
                    str += " + ";
                    break;
                case OperatorType.Take:
                    str += " - ";
                    break;
                default:
                    throw new Exception($"Unknown change armors operator type '{GetOperator()}'");
            }

            switch (GetOperandType())
            {
                case OperandType.Constant:
                    str += GetOperand();
                    break;
                case OperandType.Variable:
                    str += context.NameFormatter.FormatVariable(GetOperand());
                    break;
                default:
                    throw new Exception($"Unknown change armors operand type '{GetOperandType()}'");
            }

            if (GetIncludeEquipment())
                str += context.Version == StorageVersion.MV ? " (Include Equipment)" : ", Include Equipment";

            return str;
        }
    }
}