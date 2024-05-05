using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeGoldCommand : NamedCommand
    {
        protected const int OPERATOR_ADD = 0;
        protected const int OPERATOR_TAKE = 1;
        protected const int OPERAND_CONSTANT = 0;
        protected const int OPERAND_VARIABLE = 1;

        public int GetOperator()
        {
            return GetParameter<int>(0);
        }

        public int GetOperandType()
        {
            return GetParameter<int>(1);
        }

        public int GetOperand()
        {
            return GetParameter<int>(2);
        }

        public override int Code => CommandCodes.ChangeGold;
        public override string GetCommandName(StorageVersion version) => "Change gold";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = "";
            switch (GetOperator())
            {
                case OPERATOR_ADD:
                    str = "+";
                    break;
                case OPERATOR_TAKE:
                    str = "-";
                    break;
                default:
                    throw new Exception($"Unknown modify gold operator type '{GetOperator()}'");
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
                    throw new Exception($"Unknown modify gold operand type '{GetOperandType()}'");
            }

            return str;
        }
    }
}