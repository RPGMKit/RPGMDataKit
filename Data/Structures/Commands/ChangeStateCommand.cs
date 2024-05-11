using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeStateCommand : NamedCommand
    {
        public enum SourceType
        {
            Actor = 0,
            Variable = 1,
        }

        public enum OperatorType
        {
            Add = 0,
            Remove = 1,
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
                case SourceType.Actor:
                    str = context.NameFormatter.FormatActor(source);
                    break;
                case SourceType.Variable:
                    str = context.NameFormatter.FormatVariable(source);
                    break;
                default:
                    throw new Exception($"Unknown change state source type '{GetSourceType()}'");
            }

            switch (GetOperator())
            {
                case OperatorType.Add:
                    str += ", + ";
                    break;
                case OperatorType.Remove:
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
