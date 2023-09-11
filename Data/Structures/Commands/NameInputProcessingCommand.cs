using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class NameInputProcessingCommand : NamedCommand
    {
        public int GetActor()
        {
            return (int)GetParameter(0);
        }

        public int GetMaxCharacters()
        {
            return (int)GetParameter(1);
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
