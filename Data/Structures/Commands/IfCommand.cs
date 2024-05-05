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

        public string NameProvider = "If";

        public override int Code => CommandCodes.If;
        public override string Name => NameProvider;
    }
}
