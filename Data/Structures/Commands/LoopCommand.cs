using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class LoopCommand : NamedCommand
{
    public override int Code => CommandCodes.Loop;
    public override string GetCommandName(StorageVersion version) => "Loop";
}