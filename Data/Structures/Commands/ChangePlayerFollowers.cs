using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Commands.Abstraction;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangePlayerFollowers : AbstractOnOffCommand
{
    public override int Code => CommandCodes.ChangePlayerFollowers;
    public override string GetCommandName(StorageVersion version) => "Change Player Followers";
}