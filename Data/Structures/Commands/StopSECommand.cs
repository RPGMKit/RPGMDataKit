using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class StopSECommand : NamedCommand
{
    public override int Code => CommandCodes.StopSe;
    public override string GetCommandName(StorageVersion version) => "Stop SE";
}