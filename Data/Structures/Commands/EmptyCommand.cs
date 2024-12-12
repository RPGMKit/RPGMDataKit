using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class EmptyCommand : NamedCommand
{
    public override int Code => CommandCodes.Empty;
    public override string GetCommandName(StorageVersion version) => "";
}