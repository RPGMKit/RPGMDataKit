using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class BattleProcessingIfLoseCommand : NamedCommand
{
    public BattleProcessingIfLoseCommand()
    {
        IsSecondary = true;
    }
        
    public override int Code => CommandCodes.BattleProcessingIfLose;
    public override string GetCommandName(StorageVersion version) => "If Lose";
}