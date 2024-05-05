using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

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

        public int GetBattle()
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

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var source = GetBattle();
            var str = "";
            switch (GetDesignationType())
            {
                case DESIGNATION_TYPE_DIRECT:
                    str = "#{source}";
                    break;
                case DESIGNATION_TYPE_BY_VARIABLE:
                    str = "{#" + source;
                    var variableName = storage.GetVariable(source);
                    if (!string.IsNullOrEmpty(variableName))
                        str += $" {variableName}";

                    str += "}";
                    break;
                case DESIGNATION_TYPE_SAME_AS_RANDOM_ENCOUNTER:
                    str = "Same as Random Encounter";
                    break;
                default:
                    throw new Exception($"Unknown battle processing designation type '{GetDesignationType()}'");
            }

            if (GetCanEscape())
                str += ", Can escape";

            if (GetCanLose())
                str += ", Can lose";

            return str;
        }
    }
}