using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Commands.Abstraction;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeSaveAccessCommand : AbstractEnableAccessCommand
{
    public override int Code => CommandCodes.ChangeSaveAccess;
    public override string GetCommandName(StorageVersion version) => "Change Save Access";
}