using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class OpenMenuScreenCommand : NamedCommand
{
    public override int Code => CommandCodes.OpenMenuScreen;
    public override string GetCommandName(StorageVersion version) => "Open Menu Screen";
}