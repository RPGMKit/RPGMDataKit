using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class PlayBGSCommand : AbstractAudioCommand
    {
        public override int Code => CommandCodes.PlayBgs;
        public override string GetCommandName(StorageVersion version) => "Play BGS";
    }
}