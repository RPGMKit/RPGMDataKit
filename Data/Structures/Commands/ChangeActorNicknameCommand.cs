using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

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

    public int GetActorId() => GetParameter<int>(0);

    public string GetNickName() => GetParameter<string>(1);

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        if (context.Version == StorageVersion.MV)
            return $"{context.NameFormatter.FormatActor(GetActorId())}, {GetNickName()}";
        else
            return $"{context.NameFormatter.FormatActor(GetActorId())}, '{GetNickName()}'";
    }
}
