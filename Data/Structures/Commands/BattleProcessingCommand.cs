using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class BattleProcessingCommand : NamedCommand
    {
        protected const int DESIGNATION_TYPE_DIRECT = 0;
        protected const int DESIGNATION_TYPE_BY_VARIABLE = 1;
        protected const int DESIGNATION_TYPE_SAME_AS_RANDOM_ENCOUNTER = 2;

        public int GetDesignationType()
        {
            return GetParameter<int>(0);
        }

        public int GetSource()
        {
            return GetParameter<int>(1);
        }

        public bool GetCanEscape()
        {
            return GetParameter<bool>(2);
        }

        public bool GetCanLose()
        {
            return GetParameter<bool>(3);
        }

        public override int Code => CommandCodes.BattleProcessing;
        public override string GetCommandName(StorageVersion version) => "Battle Processing";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var source = GetSource();
            var str = "";
            switch (GetDesignationType())
            {
                case DESIGNATION_TYPE_DIRECT:
                    str = context.NameFormatter.FormatTroop(source);
                    break;
                case DESIGNATION_TYPE_BY_VARIABLE:
                    if (context.Version == StorageVersion.MV)
                        str = context.NameFormatter.FormatVariable(source, new() { BracketType = BracketType.Figure });
                    else
                        str = "Variable " + context.NameFormatter.FormatVariable(source, new() { SkipName = true });
                    break;
                case DESIGNATION_TYPE_SAME_AS_RANDOM_ENCOUNTER:
                    str = "Same as Random Encounter";
                    break;
                default:
                    throw new Exception($"Unknown battle processing designation type '{GetDesignationType()}'");
            }

            // custom
            if (GetCanEscape())
                str += ", Can escape";

            // custom
            if (GetCanLose())
                str += ", Can lose";

            return str;
        }
    }
}