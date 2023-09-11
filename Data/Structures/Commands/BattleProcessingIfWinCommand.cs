using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class BattleProcessingIfWinCommand : NamedCommand
    {
        public BattleProcessingIfWinCommand()
        {
            IsSecondary = true;
        }
        
        public override int Code => CommandCodes.BattleProcessingIfWin;
        public override string Name => "If Win";
    }
}
