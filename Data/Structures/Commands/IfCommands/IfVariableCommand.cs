using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public abstract class IfVariableCommand : IfCommand
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
    }
}
