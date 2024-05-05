using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChoiceCaseCommand : NamedCommand
    {
        public ChoiceCaseCommand()
        {
            IsSecondary = true;
        }

        public override int Code => CommandCodes.ChoiceCase;
        public override string GetCommandName(StorageVersion version) => "When";

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
