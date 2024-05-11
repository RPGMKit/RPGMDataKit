using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class BattleProcessingCommand : NamedCommand
    {
        public enum DesignationType
        {
            Direct = 0,
            ByVariable = 1,
            SameAsRandomEncounter = 2,
        }

        public DesignationType GetDesignationType()
        {
            return (DesignationType)GetParameter<int>(0);
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
                case DesignationType.Direct:
                    str = context.NameFormatter.FormatTroop(source);
                    break;
                case DesignationType.ByVariable:
                    str = context.NameFormatter.FormatVariable(source, new() { SkipName = context.Version != StorageVersion.MV });
                    break;
                case DesignationType.SameAsRandomEncounter:
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