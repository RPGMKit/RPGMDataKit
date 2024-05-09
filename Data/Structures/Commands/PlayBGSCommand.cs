using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class PlayBGSCommand : NamedCommand
    {
        public override int Code => CommandCodes.PlayBgs;
        public override string GetCommandName(StorageVersion version) => "Play BGS";

        public IAudioFile GetBGSData(StorageVersion version)
        {
            return version == StorageVersion.MV ? GetParameter<SoundData>(0) : GetParameter<IAudioFile>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var bgsData = GetBGSData(context.Version);

            if (context.Version == StorageVersion.MV)
            {
                if (string.IsNullOrWhiteSpace(bgsData.Name))
                    return "None";

                return $"{bgsData.Name} ({bgsData.Volume}, {bgsData.Pitch}, {bgsData.Pan})";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(bgsData.Name))
                    return "(None)";

                return $"'{bgsData.Name}', {bgsData.Volume}, {bgsData.Pitch}";
            }
        }
    }
}