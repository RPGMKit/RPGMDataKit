using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class FadeinScreenCommand : NamedCommand
{
    public override int Code => CommandCodes.FadeinScreen;
    public override string GetCommandName(StorageVersion version) => "Fadein Screen";
}