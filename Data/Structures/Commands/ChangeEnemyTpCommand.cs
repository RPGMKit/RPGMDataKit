using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeEnemyTpCommand : NamedCommand
    {
        protected const int OPERATOR_INCREASE = 0;
        protected const int OPERATOR_DECREASE = 1;
        protected const int OPERAND_CONSTANT = 0;
        protected const int OPERAND_VARIABLE = 1;

        public int GetEnemy()
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

        public override int Code => CommandCodes.ChangeEnemyTp;
        public override string GetCommandName(StorageVersion version) => "Change enemy TP";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var source = GetEnemy();
            var str = "";
            if (source == -1)
                str = "Entire Troop";
            else
                str = $"#{source + 1}";

            str += ", ";

            switch (GetOperator())
            {
                case OPERATOR_INCREASE:
                    str += "+";
                    break;
                case OPERATOR_DECREASE:
                    str += "-";
                    break;
                default:
                    throw new Exception($"Unknown modify enemy tp operator type '{GetOperator()}'");
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
                    throw new Exception($"Unknown modify enemy tp operand type '{GetOperandType()}'");
            }

            return str;
        }
    }
}