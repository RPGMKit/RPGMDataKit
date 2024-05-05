using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChoiceCaseCommand : NamedCommand
    {
        public ChoiceCaseCommand()
        {
            IsSecondary = true;
        }

        public override int Code => CommandCodes.ChoiceCase;
        public override string Name => "When";

        public int GetChoiceIndex()
        {
            return GetParameter<int>(0);
        }

        public string GetChoiceName()
        {
            return GetParameter<string>(1);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return $"{GetChoiceName()}";
        }
    }
}
