using RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Versions.MV.IfCommands
{
    public class MVIfVariableCommand : IfVariableCommand
    {
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
