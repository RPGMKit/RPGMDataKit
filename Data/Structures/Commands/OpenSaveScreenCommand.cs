using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class OpenSaveScreenCommand : NamedCommand
{
    public override int Code => CommandCodes.OpenSaveScreen;
    public override string GetCommandName(StorageVersion version) => "Open Save Screen";
}