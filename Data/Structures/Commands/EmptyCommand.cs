using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class EmptyCommand : NamedCommand
{
    public override int Code => CommandCodes.Empty;
    public override string GetCommandName(StorageVersion version) => "";
}