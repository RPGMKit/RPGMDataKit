using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Versions.MV
{
    public class MVPlaySECommand : PlaySECommand
    {
        public override IAudioFile GetSEData()
        {
            return GetParameter<SoundData>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var seData = GetSEData();
            if (string.IsNullOrWhiteSpace(seData.Name))
                return "None";

            return $"{seData.Name} ({seData.Volume}, {seData.Pitch}), {seData.Pan})";
        }
    }
}
