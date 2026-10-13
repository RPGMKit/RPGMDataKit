using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ElseCommand : NamedCommand
{
    public ElseCommand()
    {
        IsSecondary = true;
    }

    public override int Code => CommandCodes.Else;
    public override string GetCommandName(StorageVersion version) => "Else";
}