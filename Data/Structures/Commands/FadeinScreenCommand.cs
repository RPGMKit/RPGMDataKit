using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class FadeinScreenCommand : NamedCommand
{
    public override int Code => CommandCodes.FadeinScreen;
    public override string GetCommandName(StorageVersion version) => "Fadein Screen";
}