using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeHpCommand : NamedCommand
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

        public SourceType GetSourceType()
        {
            return (SourceType)GetParameter<int>(0);
        }

        public int GetSource()
        {
            return GetParameter<int>(1);
        }

        public OperatorType GetOperator()
        {
            return (OperatorType)GetParameter<int>(2);
        }

        public OperandType GetOperandType()
        {
            return (OperandType)GetParameter<int>(3);
        }

        public int GetOperand()
        {
            return GetParameter<int>(4);
        }

        public bool GetAllowKnockout()
        {
            return GetParameter<bool>(5);
        }

        public override int Code => CommandCodes.ChangeHp;
        public override string GetCommandName(StorageVersion version) => "Change HP";

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
                    throw new Exception($"Unknown modify hp source type '{GetSourceType()}'");
            }

            switch (GetOperator())
            {
                case OperatorType.Increase:
                    str += ", + ";
                    break;
                case OperatorType.Decrease:
                    str += ", - ";
                    break;
                default:
                    throw new Exception($"Unknown modify hp operator type '{GetOperator()}'");
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
                    throw new Exception($"Unknown modify hp operand type '{GetOperandType()}'");
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