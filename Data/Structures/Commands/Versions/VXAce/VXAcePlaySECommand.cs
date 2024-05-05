using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.VXAce.Classes;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Versions.VXAce
{
    public class VXAcePlaySECommand : PlaySECommand
    {
        public override IAudioFile GetSEData()
        {
            return GetParameter<SE>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var seData = GetSEData();
            if (string.IsNullOrWhiteSpace(seData.Name))
                return "None";

            return $"{seData.Name} ({seData.Volume}, {seData.Pitch}";
        }
    }
}
