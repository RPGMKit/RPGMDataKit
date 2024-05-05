namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfVariableCommand : IfCommand
    {
        protected const int OPERATOR_EQ = 0;
        protected const int OPERATOR_GTE = 1;
        protected const int OPERATOR_LTE = 2;
        protected const int OPERATOR_GT = 3;
        protected const int OPERATOR_LT = 4;
        protected const int OPERATOR_NEQ = 5;
        protected const int OPERAND_CONSTANT = 0;
        protected const int OPERAND_VARIABLE = 1;

        public int GetVariable()
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

        public int GetOperator()
        {
            return GetParameter<int>(4);
        }

        public void SetVariable(int value)
        {
            SetParameter(1, value);
        }

        public void SetOperand(int value)
        {
            SetParameter(3, value);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"#{GetVariable()}";
            var variableName = storage.GetVariable(GetVariable());
            if (!string.IsNullOrEmpty(variableName))
            {
                str += $" {variableName}";
            }

            switch (GetOperator())
            {
                case OPERATOR_EQ:
                    str += " = ";
                    break;
                case OPERATOR_GTE:
                    str += " >= ";
                    break;
                case OPERATOR_LTE:
                    str += " <= ";
                    break;
                case OPERATOR_GT:
                    str += " > ";
                    break;
                case OPERATOR_LT:
                    str += " < ";
                    break;
                case OPERATOR_NEQ:
                    str += " != ";
                    break;
                default:
                    throw new Exception($"Unknown operator type {GetOperator()} for '{Name}' variable command");
            }

            switch (GetOperandType())
            {
                case OPERAND_CONSTANT:
                    str += GetOperand();
                    break;
                case OPERAND_VARIABLE:
                    str += "#{" + GetOperand();
                    variableName = storage.GetVariable(GetOperand());
                    if (!string.IsNullOrEmpty(variableName))
                        str += $" {variableName}";

                    str += "}";
                    break;
                default:
                    throw new Exception(
                        $"Unknown operand type '{GetOperandType()}' for '{Name}' variable command");
            }

            return str;
        }
    }
}
