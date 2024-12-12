using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class OpenSaveScreenCommand : NamedCommand
{
    public override int Code => CommandCodes.OpenSaveScreen;
    public override string GetCommandName(StorageVersion version) => "Open Save Screen";
}