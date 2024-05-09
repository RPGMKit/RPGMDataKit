using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeMapNameDisplayCommand : AbstractOnOffCommand
{
    public override int Code => CommandCodes.ChangeMapNameDisplay;
    public override string GetCommandName(StorageVersion version) => "Change Map Name Display";
}
