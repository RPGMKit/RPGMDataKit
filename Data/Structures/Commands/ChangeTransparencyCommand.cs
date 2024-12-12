using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeTransparencyCommand : AbstractOnOffCommand
{
    public override int Code => CommandCodes.ChangeTransparency;
    public override string GetCommandName(StorageVersion version) => "Change Transparency";
}