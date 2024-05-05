using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class BattleProcessingIfEscapeCommand : NamedCommand
    {
        public BattleProcessingIfEscapeCommand()
        {
            IsSecondary = true;
        }
        
        public override int Code => CommandCodes.BattleProcessingIfEscape;
        public override string GetCommandName(StorageVersion version) => "If Escape";
    }
}
