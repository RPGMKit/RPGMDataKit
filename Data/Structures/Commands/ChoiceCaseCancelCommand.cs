using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChoiceCaseCancelCommand : NamedCommand
    {
        public ChoiceCaseCancelCommand()
        {
            IsSecondary = true;
        }

        public override int Code => CommandCodes.ChoiceCaseCancel;
        public override string Name => "When";

        public object GetUnk0()
        {
            return GetParameter(0);
        }

        public object GetUnk1()
        {
            return GetParameter(1);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return "Cancel";
        }
    }
}
