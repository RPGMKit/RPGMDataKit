using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class EndChoicesCommand : NamedCommand
{
    public EndChoicesCommand()
    {
        IsSecondary = true;
    }

    public override int Code => CommandCodes.ChoicesEnd;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "End";
        else
            return "Branch End";
    }
}