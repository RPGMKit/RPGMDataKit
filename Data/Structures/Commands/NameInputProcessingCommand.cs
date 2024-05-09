using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

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
        public override string GetCommandName(StorageVersion version) => "Name Input Processing";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var actorName = context.NameFormatter.FormatActor(GetActor(), new()
            {
                BracketType = BracketType.None
            });

            var str = $"{actorName}, {GetMaxCharacters()} character(s)";

            return str;
        }
    }
}
