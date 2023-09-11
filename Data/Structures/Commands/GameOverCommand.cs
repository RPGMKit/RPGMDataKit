using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class GameOverCommand : NamedCommand
    {
        public override int Code => CommandCodes.GameOver;
        public override string Name => "Game Over";
    }
}
