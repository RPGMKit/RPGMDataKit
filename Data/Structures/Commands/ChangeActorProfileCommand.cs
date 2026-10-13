using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeActorProfileCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeActorProfile;

    public override string GetCommandName(StorageVersion version)
    {
        return "Change Profile";
    }

    public int ActorId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public string Text
    {
        get => Get<string>(1);
        set => Set(1, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"{context.NameFormatter.FormatActor(ActorId)}, {Text}";
    }
}
