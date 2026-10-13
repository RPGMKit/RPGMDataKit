using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class CommentContinueCommand : NamedCommand
{
    public CommentContinueCommand()
    {
        IsSecondary = true;
    }

    public override int Code => CommandCodes.CommentContinue;
    public override string GetCommandName(StorageVersion version) => "       ";

    public string Comment
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Comment;
}