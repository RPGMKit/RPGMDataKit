using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class PlayBGMCommand : AbstractAudioCommand
    {
        public override int Code => CommandCodes.PlayBgm;
        public override string GetCommandName(StorageVersion version) => "Play BGM";
    }
}
