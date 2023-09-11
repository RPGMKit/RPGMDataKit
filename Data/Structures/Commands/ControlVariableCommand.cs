using Newtonsoft.Json.Linq;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ControlVariableCommand : NamedCommand
    {
        protected const int OPERATOR_SET = 0;
        protected const int OPERATOR_ADD = 1;
        protected const int OPERATOR_DEC = 2;
        protected const int OPERATOR_MUL = 3;
        protected const int OPERATOR_DIV = 4;
        protected const int OPERATOR_MOD = 5;
        protected const int OPERAND_CONSTANT = 0;
        protected const int OPERAND_VARIABLE = 1;
        protected const int OPERAND_RANDOM = 2;
        protected const int OPERAND_GAME_DATA = 3;
        protected const int OPERAND_SCENARIO = 4;

        public int GetStartVariable()
        {
            return (int)GetParameter(0);
        }

        public int GetEndVariable()
        {
            return (int)GetParameter(1);
        }

        public int GetOperator()
        {
            return (int)GetParameter(2);
        }

        public int GetOperandType()
        {
            return (int)GetParameter(3);
        }

        public JToken GetOperand1()
        {
            return GetParameter(4);
        }

        public JToken GetOperand2()
        {
            return GetParameter(5);
        }

        public override int Code => CommandCodes.ControlVariables;
        public override string Name => "Control variable";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = "";
            if (GetStartVariable() != GetEndVariable())
                str = $"#{GetStartVariable()}..{GetEndVariable()}";
            else
            {
                str = $"#{GetStartVariable()}";
                var variableName = storage.GetVariable(GetStartVariable());
                if (!string.IsNullOrEmpty(variableName))
                    str += $" {variableName}";
            }

            switch (GetOperator())
            {
                case OPERATOR_SET:
                    str += " =";
                    break;
                case OPERATOR_ADD:
                    str += " +=";
                    break;
                case OPERATOR_DEC:
                    str += " -=";
                    break;
                case OPERATOR_MUL:
                    str += " *=";
                    break;
                case OPERATOR_DIV:
                    str += " /=";
                    break;
                case OPERATOR_MOD:
                    str += " %=";
                    break;
                default:
                    throw new Exception($"Unknown operator type #{GetOperator()} for action #{Code} {Name}");
            }

            switch (GetOperandType())
            {
                case OPERAND_CONSTANT:
                    str += $" {GetOperand1()}";
                    break;
                case OPERAND_VARIABLE:
                    str += " {#" + GetOperand1();
                    var tmp = GetOperand1();
                    var varId = (int)tmp;
                    var variableName = storage.GetVariable(varId);
                    if (!string.IsNullOrEmpty(variableName))
                        str += $" {variableName}";

                    str += "}";
                    break;
                case OPERAND_RANDOM:
                    str += $" Random ({GetOperand1()}, {GetOperand2()})";
                    break;
                case OPERAND_GAME_DATA:
                    str += $" Game data ({GetOperand1()}, {GetOperand2()})";
                    break;
                case OPERAND_SCENARIO:
                    str += $" Scenario ('{GetOperand1()}', '{GetOperand2()}')";
                    break;
                default:
                    throw new Exception($"Unknown operator type #{GetOperator()} for action #{Code} {Name}");
            }

            return str;
        }
    }
}
