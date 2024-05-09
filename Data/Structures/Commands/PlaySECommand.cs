using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class PlaySECommand : AbstractAudioCommand
    {
        public override int Code => CommandCodes.PlaySe;
        public override string GetCommandName(StorageVersion version) => "Play SE";
    }
}