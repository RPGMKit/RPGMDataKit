using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeActorProfileCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeActorProfile;

    public override string GetCommandName(StorageVersion version)
    {
        return "Change Profile";
    }

    public int GetActorId() => GetParameter<int>(0);

    public string GetText() => GetParameter<string>(1);

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"{context.NameFormatter.FormatActor(GetActorId())}, {GetText()}";
    }
}
