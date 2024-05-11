using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeEnemyTpCommand : NamedCommand
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

        public int GetEnemy()
        {
            return GetParameter<int>(0);
        }

        public OperatorType GetOperator()
        {
            return (OperatorType)GetParameter<int>(1);
        }

        public OperandType GetOperandType()
        {
            return (OperandType)GetParameter<int>(2);
        }

        public int GetOperand()
        {
            return GetParameter<int>(3);
        }

        public override int Code => CommandCodes.ChangeEnemyTp;
        public override string GetCommandName(StorageVersion version) => "Change enemy TP";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatTroopMember(GetEnemy());

            switch (GetOperator())
            {
                case OperatorType.Increase:
                    str += ", + ";
                    break;
                case OperatorType.Decrease:
                    str += ", - ";
                    break;
                default:
                    throw new Exception($"Unknown modify enemy tp operator type '{GetOperator()}'");
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
                    throw new Exception($"Unknown modify enemy tp operand type '{GetOperandType()}'");
            }

            return str;
        }
    }
}