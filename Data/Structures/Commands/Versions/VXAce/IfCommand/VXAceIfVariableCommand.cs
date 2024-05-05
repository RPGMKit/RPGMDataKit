using RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Versions.VXAce.IfCommand
{
    public class VXAceIfVariableCommand : IfVariableCommand
    {
        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"Variable [{GetVariable():D4}:{storage.GetVariable(GetVariable())}]";

            switch (GetOperator())
            {
                case OPERATOR_EQ:
                    str += " == ";
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
                    str += $"Variable [{GetOperand():D4}:{storage.GetVariable(GetOperand())}]";
                    break;
                default:
                    throw new Exception(
                        $"Unknown operand type '{GetOperandType()}' for '{Name}' variable command");
            }

            return str;
        }
    }
}
