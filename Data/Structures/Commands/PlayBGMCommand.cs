using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

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

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var bgmData = GetBGMData(storage.Version);

            if (string.IsNullOrWhiteSpace(bgmData.Name))
                return "None";

            var str = $"{bgmData.Name} ({bgmData.Volume}, {bgmData.Pitch}";
            if (storage.Version == StorageVersion.MV)
                str += $", {bgmData.Pan}";
            str += ")";

            return str;
        }
    }
}