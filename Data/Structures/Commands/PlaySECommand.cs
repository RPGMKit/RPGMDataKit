using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public abstract class PlaySECommand : NamedCommand
    {
        public override int Code => CommandCodes.PlaySe;
        public override string Name => "Play SE";

        public abstract IAudioFile GetSEData();
    }
}