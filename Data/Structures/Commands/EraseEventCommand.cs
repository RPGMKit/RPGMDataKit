using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class EraseEventCommand : NamedCommand
{
    public override int Code => CommandCodes.EraseEvent;
    public override string GetCommandName(StorageVersion version) => "Erase Event";
}