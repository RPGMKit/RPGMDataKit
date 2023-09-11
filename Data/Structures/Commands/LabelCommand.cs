using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class LabelCommand : NamedCommand
    {
        public override int Code => CommandCodes.Label;
        public override string Name => "Label";

        public string GetLabel()
        {
            return (string)GetParameter(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return GetLabel();
        }
    }
}
