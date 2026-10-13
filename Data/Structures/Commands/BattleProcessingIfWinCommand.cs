using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class BattleProcessingIfWinCommand : NamedCommand
{
    public BattleProcessingIfWinCommand()
    {
        IsSecondary = true;
    }
        
    public override int Code => CommandCodes.BattleProcessingIfWin;
    public override string GetCommandName(StorageVersion version) => "If Win";
}