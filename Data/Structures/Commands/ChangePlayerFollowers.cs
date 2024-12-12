using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangePlayerFollowers : AbstractOnOffCommand
{
    public override int Code => CommandCodes.ChangePlayerFollowers;
    public override string GetCommandName(StorageVersion version) => "Change Player Followers";
}