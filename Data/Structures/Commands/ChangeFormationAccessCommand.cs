using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Commands.Abstraction;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeFormationAccessCommand : AbstractEnableAccessCommand
{
    public override int Code => CommandCodes.ChangeFormationAccess;
    public override string GetCommandName(StorageVersion version) => "Change Formation Access";
}