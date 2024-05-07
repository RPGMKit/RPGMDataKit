using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class CommentCommand : NamedCommand
    {
        public override int Code => CommandCodes.Comment;
        public override string GetCommandName(StorageVersion version) => "Comment";

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
