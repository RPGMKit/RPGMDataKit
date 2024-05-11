using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeEnemyHpCommand : NamedCommand
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
        
        public int GetMember()
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

        public bool GetAllowKnockout()
        {
            return GetParameter<bool>(4);
        }

        public override int Code => CommandCodes.ChangeEnemyHp;
        public override string GetCommandName(StorageVersion version) => "Change Enemy HP";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatTroopMember(GetMember());

            switch (GetOperator())
            {
                case OperatorType.Increase:
                    str += ", + ";
                    break;
                case OperatorType.Decrease:
                    str += ", - ";
                    break;
                default:
                    throw new Exception($"Unknown modify enemy hp operator type '{GetOperator()}'");
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
                    throw new Exception($"Unknown modify enemy hp operand type '{GetOperandType()}'");
            }

            if (GetAllowKnockout())
            {
                if (context.Version == StorageVersion.MV)
                    str += " (Allow Knockout)";
                else
                    // custom
                    str += ", Allow Knockout";
            }

            return str;
        }
    }
}