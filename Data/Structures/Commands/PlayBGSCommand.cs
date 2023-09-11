using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class PlayBGSCommand : NamedCommand
    {
        public override int Code => CommandCodes.PlayBgs;
        public override string Name => "Play BGS";

        public SoundData GetBGSData()
        {
            return GetParameter(0)?.ToObject<SoundData>();
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