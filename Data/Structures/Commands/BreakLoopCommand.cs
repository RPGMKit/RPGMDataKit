using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class BreakLoopCommand : NamedCommand
{
    public override int Code => CommandCodes.BreakLoop;
    public override string GetCommandName(StorageVersion version) => "Break Loop";
}