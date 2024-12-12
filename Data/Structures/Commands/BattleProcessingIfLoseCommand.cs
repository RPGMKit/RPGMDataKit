using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class BattleProcessingIfLoseCommand : NamedCommand
{
    public BattleProcessingIfLoseCommand()
    {
        IsSecondary = true;
    }
        
    public override int Code => CommandCodes.BattleProcessingIfLose;
    public override string GetCommandName(StorageVersion version) => "If Lose";
}