using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class EraseEventCommand : NamedCommand
{
    public override int Code => CommandCodes.EraseEvent;
    public override string GetCommandName(StorageVersion version) => "Erase Event";
}