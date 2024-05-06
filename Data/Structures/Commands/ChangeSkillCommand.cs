using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

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

        public int GetSkillId()
        {
            return GetParameter<int>(3);
        }

        public override int Code => CommandCodes.ChangeSkill;

        public override string GetCommandName(StorageVersion version)
        {
            if (version == StorageVersion.MV)
                return "Change skill";
            else
                return "Change Skills";
        }

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
                        str = storage.NameFormatter.FormatActor(source);
                    break;
                case SOURCE_TYPE_VARIABLE:
                    if (storage.Version == StorageVersion.MV)
                        str = "{" + storage.NameFormatter.FormatVariable(source) + "}";
                    else
                        str = $"Variable {storage.NameFormatter.FormatVariable(source)}";
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

            str += $" {storage.NameFormatter.FormatSkill(GetSkillId())}";

            return str;
        }
    }
}
