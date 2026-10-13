using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Commands.Abstraction;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeMenuAccessCommand : AbstractEnableAccessCommand
{
    public override int Code => CommandCodes.ChangeMenuAccess;
    public override string GetCommandName(StorageVersion version) => "Change Menu Access";
}