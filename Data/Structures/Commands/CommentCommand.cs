using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class CommentCommand : NamedCommand
    {
        public override int Code => CommandCodes.Comment;
        public override string Name => "Comment";

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
