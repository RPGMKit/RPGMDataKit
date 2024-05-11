using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeSkillCommand : NamedCommand
    {
        public enum SourceType
        {
            Actor = 0,
            Variable = 1,
        }
        
        public enum OperatorType
        {
            Learn = 0,
            Forget = 1,
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

        public int GetSkillId()
        {
            return GetParameter<int>(3);
        }

        public override int Code => CommandCodes.ChangeSkill;

        public override string GetCommandName(StorageVersion version)
        {
            if (version == StorageVersion.MV)
                return "Change Skill";
            else
                return "Change Skills";
        }

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
                    throw new Exception($"Unknown change skill source type '{GetSourceType()}'");
            }

            switch (GetOperator())
            {
                case OperatorType.Learn:
                    str += ", + ";
                    break;
                case OperatorType.Forget:
                    str += ", - ";
                    break;
                default:
                    throw new Exception($"Unknown change skill operator type '{GetOperator()}'");
            }

            str += context.NameFormatter.FormatSkill(GetSkillId());

            return str;
        }
    }
}
