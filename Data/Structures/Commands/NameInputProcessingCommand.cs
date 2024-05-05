using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class NameInputProcessingCommand : NamedCommand
    {
        public int GetActor()
        {
            return GetParameter<int>(0);
        }

        public int GetMaxCharacters()
        {
            return GetParameter<int>(1);
        }

        public override int Code => CommandCodes.NameInputProcessing;
        public override string Name => "Name Input Processing";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"#{GetActor()}, {GetMaxCharacters()} character(s)";

            return str;
        }
    }
}
