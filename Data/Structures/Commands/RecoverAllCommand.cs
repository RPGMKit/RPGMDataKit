using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class RecoverAllCommand : NamedCommand
    {
        protected const int SOURCE_TYPE_ACTOR = 0;
        protected const int SOURCE_TYPE_VARIABLE = 1;

        public int GetSourceType()
        {
            return GetParameter<int>(0);
        }

        public int GetSource()
        {
            return GetParameter<int>(1);
        }

        public override int Code => CommandCodes.RecoverAll;
        public override string GetCommandName(StorageVersion version) => "Recover all";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
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
                    var variableName = context.Storage.GetVariable(source);
                    if (!string.IsNullOrEmpty(variableName))
                        str += $" {variableName}";
                    str += "}";
                    break;
                default:
                    throw new Exception($"Unknown recover all source type '{GetSourceType()}'");
            }

            return str;
        }
    }
}
