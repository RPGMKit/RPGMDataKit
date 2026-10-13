using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class BreakLoopCommand : NamedCommand
{
    public override int Code => CommandCodes.BreakLoop;
    public override string GetCommandName(StorageVersion version) => "Break Loop";
}