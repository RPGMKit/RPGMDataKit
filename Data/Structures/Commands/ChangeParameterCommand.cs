using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeParameterCommand : NamedCommand
    {
        protected const int SOURCE_TYPE_ACTOR = 0;
        protected const int SOURCE_TYPE_VARIABLE = 1;
        protected const int OPERATOR_INCREASE = 0;
        protected const int OPERATOR_DECREASE = 1;
        protected const int OPERAND_CONSTANT = 0;
        protected const int OPERAND_VARIABLE = 1;

        public int GetSourceType()
        {
            return GetParameter<int>(0);
        }

        public int GetSource()
        {
            return GetParameter<int>(1);
        }

        public int GetActionParameter()
        {
            return GetParameter<int>(2);
        }

        public int GetOperator()
        {
            return GetParameter<int>(3);
        }

        public int GetOperandType()
        {
            return GetParameter<int>(4);
        }

        public int GetOperand()
        {
            return GetParameter<int>(5);
        }

        public override int Code => CommandCodes.ChangeParameter;
        public override string GetCommandName(StorageVersion version) => "Change parameter";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var source = GetSource();
            var str = "";
            switch (GetSourceType())
            {
                case SOURCE_TYPE_ACTOR:
                    if (source == 0)
                        str = "Entire party";
                    else
                        str = $"#{GetSource()}";

                    break;
                case SOURCE_TYPE_VARIABLE:
                    str = "{#" + source;
                    var variableName = context.Storage.GetVariable(source);
                    if (!string.IsNullOrEmpty(variableName))
                        str += $" {variableName}";
                    str += "}";
                    break;
                default:
                    throw new Exception($"Unknown change parameter source type '{GetSourceType()}'");
            }

            str += $", #{GetActionParameter()} ";

            switch (GetOperator())
            {
                case OPERATOR_INCREASE:
                    str += "+";
                    break;
                case OPERATOR_DECREASE:
                    str += "-";
                    break;
                default:
                    throw new Exception($"Unknown change parameter operator type '{GetOperator()}'");
            }

            str += " ";

            switch (GetOperandType())
            {
                case OPERAND_CONSTANT:
                    str += GetOperand();
                    break;
                case OPERAND_VARIABLE:
                    str += "{#" + GetOperand();
                    var variableName = context.Storage.GetVariable(GetOperand());
                    if (!string.IsNullOrEmpty(variableName))
                    {
                        str += $" {variableName}";
                    }

                    str += "}";
                    break;
                default:
                    throw new Exception($"Unknown change parameter operand type '{GetOperandType()}'");
            }

            return str;
        }
    }
}
