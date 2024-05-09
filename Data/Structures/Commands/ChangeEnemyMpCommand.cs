using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeEnemyMpCommand : NamedCommand
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

        public override int Code => CommandCodes.ChangeEnemyMp;
        public override string GetCommandName(StorageVersion version) => "Change Enemy MP";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatTroopMember(GetEnemy());

            switch (GetOperator())
            {
                case OPERATOR_INCREASE:
                    str += ", + ";
                    break;
                case OPERATOR_DECREASE:
                    str += ", - ";
                    break;
                default:
                    throw new Exception($"Unknown modify enemy mp operator type '{GetOperator()}'");
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
                    throw new Exception($"Unknown modify enemy mp operand type '{GetOperandType()}'");
            }

            return str;
        }
    }
}
