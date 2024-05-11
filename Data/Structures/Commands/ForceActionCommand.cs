using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ForceActionCommand : NamedCommand
    {
        public enum SubjectType
        {
            Enemy = 0,
            Actor = 1,
        }

        public enum TargetType
        {
            LastTarget = -2,
            Random = -1,
            Index1 = 0,
            Index2 = 1,
            Index3 = 2,
            Index4 = 3,
            Index5 = 4,
            Index6 = 5,
            Index7 = 6,
            Index8 = 7,
        }

        public override int Code => CommandCodes.ForceAction;
        public override string GetCommandName(StorageVersion version) => "Force Action";

        public SubjectType GetSubjectType()
        {
            return (SubjectType)GetParameter<int>(0);
        }

        public int GetSubject()
        {
            return GetParameter<int>(1);
        }
        
        public int GetSkill()
        {
            return GetParameter<int>(2);
        }
        
        public TargetType GetTarget()
        {
            return (TargetType)GetParameter<int>(3);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            string str;
            var subjectType = GetSubjectType();
            if (subjectType == SubjectType.Enemy)
                str = context.NameFormatter.FormatTroopMember(GetSubject());
            else if (subjectType == SubjectType.Actor)
                str = context.NameFormatter.FormatActor(GetSubject());
            else
                throw new Exception($"Unknown subject type {subjectType}");
            
            str += $", {context.NameFormatter.FormatSkill(GetSkill())}";

            var targetIndex = GetTarget();
            switch (targetIndex)
            {
                case TargetType.LastTarget:
                    str += ", Last Target";
                    break;
                case TargetType.Random:
                    str += ", Random";
                    break;
                default:
                    str += $", Index {(int)targetIndex + 1}";
                    break;
            }

            return str;
        }
    }
}
