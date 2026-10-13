using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeActorNicknameCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeActorNickname;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Change Nickname";
        else
            return "Change Actor Nickname";
    }

    public int ActorId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public string NickName
    {
        get => Get<string>(1);
        set => Set(1, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        if (context.Version == StorageVersion.MV)
            return $"{context.NameFormatter.FormatActor(ActorId)}, {NickName}";
        else
            return $"{context.NameFormatter.FormatActor(ActorId)}, '{NickName}'";
    }
}
