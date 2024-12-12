using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class StopSECommand : NamedCommand
{
    public override int Code => CommandCodes.StopSE;
    public override string GetCommandName(StorageVersion version) => "Stop SE";
}