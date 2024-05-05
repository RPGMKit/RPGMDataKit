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

        public T GetUnk0<T>()
        {
            return GetParameter<T>(0);
        }

        public T GetUnk1<T>()
        {
            return GetParameter<T>(1);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return "Cancel";
        }
    }
}
