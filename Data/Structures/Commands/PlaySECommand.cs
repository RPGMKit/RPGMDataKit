using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.VXAce.Classes;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class PlaySECommand : NamedCommand
    {
        public override int Code => CommandCodes.PlaySe;
        public override string GetCommandName(StorageVersion version) => "Play SE";

        public IAudioFile GetSEData(StorageVersion version)
        {
            return version == StorageVersion.MV ? GetParameter<SoundData>(0) : GetParameter<SE>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var seData = GetSEData(storage.Version);
            
            if (storage.Version == StorageVersion.VXAce)
            {
                if (string.IsNullOrWhiteSpace(seData.Name))
                    return "(None)";
                
                return  $"'{seData.Name}', {seData.Volume}, {seData.Pitch}";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(seData.Name))
                    return "None";
                
                return $"{seData.Name} ({seData.Volume}, {seData.Pitch}), {seData.Pan})";
            }
        }
    }
}