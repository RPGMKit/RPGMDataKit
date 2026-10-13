using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class BattleProcessingIfEscapeCommand : NamedCommand
{
    public BattleProcessingIfEscapeCommand()
    {
        IsSecondary = true;
    }
        
    public override int Code => CommandCodes.BattleProcessingIfEscape;
    public override string GetCommandName(StorageVersion version) => "If Escape";
}