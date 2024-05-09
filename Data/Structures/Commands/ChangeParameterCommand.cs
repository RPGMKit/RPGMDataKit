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

        private static readonly Dictionary<int, Tuple<string,string>> ParameterNamesMap = new()
        {
            [0] = new("Max HP", "MHP"),
            [1] = new("Max MP", "MMP"),
            [2] = new("Attack","ATK"),
            [3] = new("Defense", "DEF"),
            [4] = new("M.Attack","MAT"),
            [5] = new("M.Defense", "MDF"),
            [6] = new("Agility", "AGI"),
            [7] = new("Luck", "LUK"),
        };
        
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

        public override string GetCommandName(StorageVersion version)
        {
            if (version == StorageVersion.MV)
                return "Change Parameter";
            else
                return "Change Parameters";
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var source = GetSource();
            var str = "";
            switch (GetSourceType())
            {
                case SOURCE_TYPE_ACTOR:
                    str = context.NameFormatter.FormatActor(source);
                    break;
                case SOURCE_TYPE_VARIABLE:
                    str = context.NameFormatter.FormatVariable(source,
                        new() { BracketType = context.Version == StorageVersion.MV ? BracketType.Figure : null });
                    break;
                default:
                    throw new Exception($"Unknown change parameter source type '{GetSourceType()}'");
            }

            if (!ParameterNamesMap.TryGetValue(GetActionParameter(), out var actionName))
                throw new Exception($"Unknown parameter type {GetActionParameter()} for change parameter command");
            
            str += ", " + (context.Version == StorageVersion.MV ? actionName.Item1 : actionName.Item2);

            switch (GetOperator())
            {
                case OPERATOR_INCREASE:
                    str += " + ";
                    break;
                case OPERATOR_DECREASE:
                    str += " - ";
                    break;
                default:
                    throw new Exception($"Unknown change parameter operator type '{GetOperator()}'");
            }

            switch (GetOperandType())
            {
                case OPERAND_CONSTANT:
                    str += GetOperand();
                    break;
                case OPERAND_VARIABLE:
                    str += context.NameFormatter.FormatVariable(GetOperand(),
                        new() { BracketType = context.Version == StorageVersion.MV ? BracketType.Figure : null });
                    break;
                default:
                    throw new Exception($"Unknown change parameter operand type '{GetOperandType()}'");
            }

            return str;
        }
    }
}
