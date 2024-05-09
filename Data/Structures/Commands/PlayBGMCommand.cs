using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class PlayBGMCommand : NamedCommand
    {
        public override int Code => CommandCodes.PlayBgm;
        public override string GetCommandName(StorageVersion version) => "Play BGM";

        public IAudioFile GetBGMData(StorageVersion version)
        {
            return version == StorageVersion.MV ? GetParameter<SoundData>(0) : GetParameter<IAudioFile>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var bgmData = GetBGMData(context.Version);

            if (context.Version == StorageVersion.MV)
            {
                if (string.IsNullOrWhiteSpace(bgmData.Name))
                    return "None";

                return $"{bgmData.Name} ({bgmData.Volume}, {bgmData.Pitch}, {bgmData.Pan})";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(bgmData.Name))
                    return "(None)";

                return $"'{bgmData.Name}', {bgmData.Volume}, {bgmData.Pitch}";
            }
        }
    }
}
