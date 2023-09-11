using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ForceActionCommand : NamedCommand
    {
        private const int SUBJECT_TYPE_ENEMY = 0; 
        private const int SUBJECT_TYPE_ACTOR = 1;

        private const int TARGET_LAST_TARGET = -2;
        private const int TARGET_RANDOM = -1;
        
        public override int Code => CommandCodes.ForceAction;
        public override string Name => "Force Action";

        public int GetSubjectType()
        {
            return (int)GetParameter(0);
        }

        public int GetSubject()
        {
            return (int)GetParameter(1);
        }
        
        public int GetSkill()
        {
            return (int)GetParameter(2);
        }
        
        public int GetTarget()
        {
            return (int)GetParameter(3);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = "";
            if (GetSubjectType() == SUBJECT_TYPE_ENEMY)
                str += $"#{GetSubject() + 1}";
            else
                str += $"#{GetSubject()}";
            
            str += $", #{GetSkill()}, ";

            switch (GetTarget())
            {
                case TARGET_LAST_TARGET:
                    str += "Last Target";
                    break;
                case TARGET_RANDOM:
                    str += "Random";
                    break;
                default:
                    str += $"Index {GetTarget() + 1}";
                    break;
            }

            return str;
        }
    }
}
