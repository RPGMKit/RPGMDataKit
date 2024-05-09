using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeHpCommand : NamedCommand
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
                case SOURCE_TYPE_ACTOR:
                    str = context.NameFormatter.FormatActor(source);
                    break;
                case SOURCE_TYPE_VARIABLE:
                    str = context.NameFormatter.FormatVariable(source,
                        new() { BracketType = context.Version == StorageVersion.MV ? BracketType.Figure : null });
                    break;
                default:
                    throw new Exception($"Unknown modify hp source type '{GetSourceType()}'");
            }

            switch (GetOperator())
            {
                case OPERATOR_INCREASE:
                    str += ", + ";
                    break;
                case OPERATOR_DECREASE:
                    str += ", - ";
                    break;
                default:
                    throw new Exception($"Unknown modify hp operator type '{GetOperator()}'");
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