using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class PlayBGSCommand : NamedCommand
    {
        public override int Code => CommandCodes.PlayBgs;
        public override string GetCommandName(StorageVersion version) => "Play BGS";

        public SoundData GetBGSData()
        {
            return GetParameter<SoundData>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var bgmData = GetBGSData();

            if (string.IsNullOrWhiteSpace(bgmData.Name))
                return "None";

            return $"{bgmData.Name} ({bgmData.Volume}, {bgmData.Pitch}, {bgmData.Pan})";
        }
    }
}