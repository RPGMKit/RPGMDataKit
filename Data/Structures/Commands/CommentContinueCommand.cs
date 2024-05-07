using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class CommentContinueCommand : NamedCommand
    {
        public CommentContinueCommand()
        {
            IsSecondary = true;
        }

        public override int Code => CommandCodes.CommentContinue;
        public override string GetCommandName(StorageVersion version) => "       ";

        public string GetComment()
        {
            return GetParameter<string>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return GetComment();
        }
    }
}