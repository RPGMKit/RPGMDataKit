using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Commands.Abstraction;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeTransparencyCommand : AbstractOnOffCommand
{
    public override int Code => CommandCodes.ChangeTransparency;
    public override string GetCommandName(StorageVersion version) => "Change Transparency";
}