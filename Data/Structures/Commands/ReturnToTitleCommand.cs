using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ReturnToTitleCommand : NamedCommand
{
    public override int Code => CommandCodes.ReturnToTitle;
    public override string GetCommandName(StorageVersion version) => "Return to Title Screen";
}