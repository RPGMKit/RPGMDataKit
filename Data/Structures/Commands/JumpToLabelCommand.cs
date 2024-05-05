using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class JumpToLabelCommand : NamedCommand
    {
        public override int Code => CommandCodes.JumpToLabel;
        public override string Name => "Jump to label";

        public string GetLabel()
        {
            return GetParameter<string>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return GetLabel();
        }
    }
}
