using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeStateCommand : NamedCommand
    {
        protected const int SOURCE_TYPE_ACTOR = 0;
        protected const int SOURCE_TYPE_VARIABLE = 1;
        protected const int OPERATOR_ADD = 0;
        protected const int OPERATOR_REMOVE = 1;

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

        public int GetState()
        {
            return GetParameter<int>(3);
        }

        public override int Code => CommandCodes.ChangeState;
        public override string GetCommandName(StorageVersion version) => "Change State";

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
                    throw new Exception($"Unknown change state source type '{GetSourceType()}'");
            }

            switch (GetOperator())
            {
                case OPERATOR_ADD:
                    str += ", + ";
                    break;
                case OPERATOR_REMOVE:
                    str += ", - ";
                    break;
                default:
                    throw new Exception($"Unknown modify state operator type '{GetOperator()}'");
            }

            str += context.NameFormatter.FormatState(GetState());

            return str;
        }
    }
}
