using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

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
            return (int)GetParameter(0);
        }

        public int GetOperandType()
        {
            return (int)GetParameter(1);
        }

        public int GetOperand()
        {
            return (int)GetParameter(2);
        }

        public override int Code => CommandCodes.ChangeGold;
        public override string Name => "Change gold";

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