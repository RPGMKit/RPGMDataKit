using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfVariableCommand : IfCommand
    {
        public enum OperatorType
        {
            Eq = 0,
            Gte = 1,
            Lte = 2,
            Gt = 3,
            Lt = 4,
            NEq = 5,
        }

        public enum OperandType
        {
            Constant = 0,
            Variable = 1,
        }

        public int GetVariable()
        {
            return GetParameter<int>(1);
        }

        public OperandType GetOperandType()
        {
            return (OperandType)GetParameter<int>(2);
        }

        public int GetOperand()
        {
            return GetParameter<int>(3);
        }

        public OperatorType GetOperator()
        {
            return (OperatorType)GetParameter<int>(4);
        }

        public void SetVariable(int value)
        {
            SetParameter(1, value);
        }

        public void SetOperand(int value)
        {
            SetParameter(3, value);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatVariable(GetVariable(), new()
            {
                BracketType = context.Version == StorageVersion.MV ? BracketType.None : null
            });

            switch (GetOperator())
            {
                case OperatorType.Eq:
                    str += context.Version == StorageVersion.MV ? " = " : " == ";
                    break;
                case OperatorType.Gte:
                    str += context.Version == StorageVersion.MV ? " ≥ " : " >= ";
                    break;
                case OperatorType.Lte:
                    str += context.Version == StorageVersion.MV ? " ≤ " : " <= ";
                    break;
                case OperatorType.Gt:
                    str += " > ";
                    break;
                case OperatorType.Lt:
                    str += " < ";
                    break;
                case OperatorType.NEq:
                    str += context.Version == StorageVersion.MV ? " ≠ " : " != ";
                    break;
                default:
                    throw new Exception(
                        $"Unknown operator type {GetOperator()} for '{GetCommandName(context.Version)}' variable command");
            }

            switch (GetOperandType())
            {
                case OperandType.Constant:
                    str += GetOperand();
                    break;
                case OperandType.Variable:
                    str += context.NameFormatter.FormatVariable(GetOperand(), new()
                    {
                        BracketType = context.Version == StorageVersion.MV ? BracketType.None : null
                    });
                    break;
                default:
                    throw new Exception(
                        $"Unknown operand type '{GetOperandType()}' for '{GetCommandName(context.Version)}' variable command");
            }

            return str;
        }
    }
}
