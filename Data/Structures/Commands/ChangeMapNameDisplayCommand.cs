using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Commands.Abstraction;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeMapNameDisplayCommand : AbstractOnOffCommand
{
    public override int Code => CommandCodes.ChangeMapNameDisplay;
    public override string GetCommandName(StorageVersion version) => "Change Map Name Display";
}
