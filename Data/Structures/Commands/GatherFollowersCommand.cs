using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class GatherFollowersCommand : NamedCommand
{
    public override int Code => CommandCodes.GatherFollowers;
    public override string GetCommandName(StorageVersion version) => "Gather Followers";
}