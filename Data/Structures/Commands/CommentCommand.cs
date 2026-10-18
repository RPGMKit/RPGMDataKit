using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class CommentCommand : NamedCommand
{
    public override int Code => CommandCodes.Comment;
    public override string GetCommandName(StorageVersion version) => "Comment";

    [EditorDefault("")]
    public string Comment
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Comment;
}