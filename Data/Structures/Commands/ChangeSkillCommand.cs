using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeSkillCommand : NamedCommand
    {
        protected const int SOURCE_TYPE_ACTOR = 0;
        protected const int SOURCE_TYPE_VARIABLE = 1;
        protected const int OPERATOR_LEARN = 0;
        protected const int OPERATOR_FORGET = 1;

        public int GetSourceType()
        {
            return (int)GetParameter(0);
        }

        public int GetSource()
        {
            return (int)GetParameter(1);
        }

        public int GetOperator()
        {
            return (int)GetParameter(2);
        }

        public int GetSkill()
        {
            return (int)GetParameter(3);
        }

        public override int Code => CommandCodes.ChangeSkill;
        public override string Name => "Change skill";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var source = GetSource();
            var str = "";
            switch (GetSourceType())
            {
                case SOURCE_TYPE_ACTOR:
                    if (source == 0)
                        str = "Entire party";
                    else
                        str = $"#{GetSource()}";

                    break;
                case SOURCE_TYPE_VARIABLE:
                    str = "{#" + source;
                    var variableName = storage.GetVariable(source);
                    if (!string.IsNullOrEmpty(variableName))
                        str += $" {variableName}";
                    str += "}";
                    break;
                default:
                    throw new Exception($"Unknown change skill source type '{GetSourceType()}'");
            }

            switch (GetOperator())
            {
                case OPERATOR_LEARN:
                    str += ", +";
                    break;
                case OPERATOR_FORGET:
                    str += ", -";
                    break;
                default:
                    throw new Exception($"Unknown change skill operator type '{GetOperator()}'");
            }

            str += $" #{GetSkill()}";

            return str;
        }
    }
}
