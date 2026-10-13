using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class EndIfCommand : NamedCommand
{
    public EndIfCommand()
    {
        IsSecondary = true;
    }

    public override int Code => CommandCodes.EndIf;

    public override string GetCommandName(StorageVersion version)
    {
        return version == StorageVersion.MV ? "End" : "Branch End";
    }
}