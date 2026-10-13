using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class GatherFollowersCommand : NamedCommand
{
    public override int Code => CommandCodes.GatherFollowers;
    public override string GetCommandName(StorageVersion version) => "Gather Followers";
}