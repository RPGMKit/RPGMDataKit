using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public abstract class IfCommand : NamedCommand
    {
        public int GetIfType()
        {
            return GetParameter<int>(0);
        }

        public override string GetCommandName(StorageVersion version)
        {
            return version == StorageVersion.MV ? "If" : "Conditional Branch";
        }

        public override int Code => CommandCodes.If;
    }
}
