using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ShowAnimationCommand : NamedCommand
{
    public override int Code => CommandCodes.ShowAnimation;
    public override string GetCommandName(StorageVersion version) => "Show Animation";

    public int EventId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public int AnimationId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public bool WaitForCompletion
    {
        get => Get<bool>(2);
        set => Set(2, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatEvent(EventId);

        str += ", " + context.NameFormatter.FormatAnimation(AnimationId);

        if (WaitForCompletion)
            str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

        return str;
    }
}