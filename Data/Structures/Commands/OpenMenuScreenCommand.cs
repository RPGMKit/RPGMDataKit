using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class OpenMenuScreenCommand : NamedCommand
{
    public override int Code => CommandCodes.OpenMenuScreen;
    public override string GetCommandName(StorageVersion version) => "Open Menu Screen";
}