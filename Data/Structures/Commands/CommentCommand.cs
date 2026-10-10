using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class CommentCommand : NamedCommand
{
    public override int Code => CommandCodes.Comment;
    public override string GetCommandName(StorageVersion version) => "Comment";

    public string Comment
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Comment;
}