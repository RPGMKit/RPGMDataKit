using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class PlaySECommand : NamedCommand
    {
        public override int Code => CommandCodes.PlaySe;
        public override string GetCommandName(StorageVersion version) => "Play SE";

        public IAudioFile GetSEData(StorageVersion version)
        {
            return version == StorageVersion.MV ? GetParameter<SoundData>(0) : GetParameter<IAudioFile>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var seData = GetSEData(context.Version);
            
            if (context.Version == StorageVersion.MV)
            {
                if (string.IsNullOrWhiteSpace(seData.Name))
                    return "None";

                return $"{seData.Name} ({seData.Volume}, {seData.Pitch}, {seData.Pan})";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(seData.Name))
                    return "(None)";

                return $"'{seData.Name}', {seData.Volume}, {seData.Pitch}";
            }
        }
    }
}