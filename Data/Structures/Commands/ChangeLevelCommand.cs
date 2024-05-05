using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeLevelCommand : NamedCommand
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

        public int GetOperator()
        {
            return GetParameter<int>(2);
        }

        public int GetOperandType()
        {
            return GetParameter<int>(3);
        }

        public int GetOperand()
        {
            return GetParameter<int>(4);
        }

        public bool GetShowLevelup()
        {
            return GetParameter<bool>(5);
        }

        public override int Code => CommandCodes.ChangeLevel;
        public override string GetCommandName(StorageVersion version) => "Change level";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
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
                    var variableName = storage.GetVariable(source);
                    if (!string.IsNullOrEmpty(variableName))
                        str += $" {variableName}";
                    str += "}";
                    break;
                default:
                    throw new Exception($"Unknown change level source type '{GetSourceType()}'");
            }

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
                    throw new Exception($"Unknown change level operator type '{GetOperator()}'");
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
                    throw new Exception($"Unknown change level operand type '{GetOperandType()}'");
            }

            if (GetShowLevelup())
                str += " (Show Level Up)";

            return str;
        }
    }
}