using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class LabelCommand : NamedCommand
    {
        public override int Code => CommandCodes.Label;
        public override string GetCommandName(StorageVersion version) => "Label";

        public string GetLabel()
        {
            return GetParameter<string>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return GetLabel();
        }
    }
}
