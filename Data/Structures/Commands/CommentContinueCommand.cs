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
        public override string Name => "       ";

        public string GetComment()
        {
            return (string)GetParameter(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return GetComment();
        }
    }
}