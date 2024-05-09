using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeTpCommand : NamedCommand
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

        public override int Code => CommandCodes.ChangeTp;
        public override string GetCommandName(StorageVersion version) => "Change TP";

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
                    throw new Exception($"Unknown modify tp source type '{GetSourceType()}'");
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
                    throw new Exception($"Unknown modify tp operator type '{GetOperator()}'");
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
                    throw new Exception($"Unknown modify tp operand type '{GetOperandType()}'");
            }

            return str;
        }
    }
}
