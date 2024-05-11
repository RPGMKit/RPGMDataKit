using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeParameterCommand : NamedCommand
    {
        public enum SourceType
        {
            Actor = 0,
            Variable = 1,
        }

        public enum OperatorType
        {
            Increase = 0,
            Decrease = 1,
        }

        public enum OperandType
        {
            Constant = 0,
            Variable = 1,
        }

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
        
        public SourceType GetSourceType()
        {
            return (SourceType)GetParameter<int>(0);
        }

        public int GetSource()
        {
            return GetParameter<int>(1);
        }

        public int GetActionParameter()
        {
            return GetParameter<int>(2);
        }

        public OperatorType GetOperator()
        {
            return (OperatorType)GetParameter<int>(3);
        }

        public OperandType GetOperandType()
        {
            return (OperandType)GetParameter<int>(4);
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
                case SourceType.Actor:
                    str = context.NameFormatter.FormatActor(source);
                    break;
                case SourceType.Variable:
                    str = context.NameFormatter.FormatVariable(source);
                    break;
                default:
                    throw new Exception($"Unknown change parameter source type '{GetSourceType()}'");
            }

            if (!ParameterNamesMap.TryGetValue(GetActionParameter(), out var actionName))
                throw new Exception($"Unknown parameter type {GetActionParameter()} for change parameter command");
            
            str += ", " + (context.Version == StorageVersion.MV ? actionName.Item1 : actionName.Item2);

            switch (GetOperator())
            {
                case OperatorType.Increase:
                    str += " + ";
                    break;
                case OperatorType.Decrease:
                    str += " - ";
                    break;
                default:
                    throw new Exception($"Unknown change parameter operator type '{GetOperator()}'");
            }

            switch (GetOperandType())
            {
                case OperandType.Constant:
                    str += GetOperand();
                    break;
                case OperandType.Variable:
                    str += context.NameFormatter.FormatVariable(GetOperand());
                    break;
                default:
                    throw new Exception($"Unknown change parameter operand type '{GetOperandType()}'");
            }

            return str;
        }
    }
}
