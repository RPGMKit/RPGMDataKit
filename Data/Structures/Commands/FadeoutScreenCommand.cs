using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class FadeoutScreenCommand : NamedCommand
{
    public override int Code => CommandCodes.FadeoutScreen;
    public override string GetCommandName(StorageVersion version) => "Fadeout Screen";
}