using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class PlayMECommand : AbstractAudioCommand
    {
        public override int Code => CommandCodes.PlayMe;
        public override string GetCommandName(StorageVersion version) => "Play ME";
    }
}