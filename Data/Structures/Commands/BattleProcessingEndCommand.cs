using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class BattleProcessingEndCommand : NamedCommand
{
    public BattleProcessingEndCommand()
    {
        IsSecondary = true;
    }

    public override int Code => CommandCodes.BattleProcessingEnd;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "End";
        else
            return "Branch End";
    }
}