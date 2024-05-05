using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class PlayBGMCommand : NamedCommand
    {
        public override int Code => CommandCodes.PlayBgm;
        public override string Name => "Play BGM";

        public SoundData GetBGMData()
        {
            return GetParameter<SoundData>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var bgmData = GetBGMData();

            if (string.IsNullOrWhiteSpace(bgmData.Name))
                return "None";

            return $"{bgmData.Name} ({bgmData.Volume}, {bgmData.Pitch}, {bgmData.Pan})";
        }
    }
}