using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ExitEventProcessingCommand : NamedCommand
{
    public override int Code => CommandCodes.ExitEventProcessing;
    public override string GetCommandName(StorageVersion version) => "Exit Event Processing";
}