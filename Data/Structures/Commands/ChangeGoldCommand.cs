using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeGoldCommand : NamedCommand
    {
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

        public OperatorType GetOperator()
        {
            return (OperatorType)GetParameter<int>(0);
        }

        public OperandType GetOperandType()
        {
            return (OperandType)GetParameter<int>(1);
        }

        public int GetOperand()
        {
            return GetParameter<int>(2);
        }

        public override int Code => CommandCodes.ChangeGold;
        public override string GetCommandName(StorageVersion version) => "Change gold";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = "";
            switch (GetOperator())
            {
                case OperatorType.Increase:
                    str = "+ ";
                    break;
                case OperatorType.Decrease:
                    str = "- ";
                    break;
                default:
                    throw new Exception($"Unknown modify gold operator type '{GetOperator()}'");
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
                    throw new Exception($"Unknown modify gold operand type '{GetOperandType()}'");
            }

            return str;
        }
    }
}