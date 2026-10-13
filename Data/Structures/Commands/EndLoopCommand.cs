using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class EndLoopCommand : NamedCommand
{
    public EndLoopCommand()
    {
        IsSecondary = true;
    }

    public override int Code => CommandCodes.EndLoop;
    public override string GetCommandName(StorageVersion version) => "Repeat Above";
}