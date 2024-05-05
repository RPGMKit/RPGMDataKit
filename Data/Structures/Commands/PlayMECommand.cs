using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class PlayMECommand : NamedCommand
    {
        public override int Code => CommandCodes.PlayMe;
        public override string Name => "Play ME";

        public SoundData GetSEData()
        {
            return GetParameter<SoundData>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var seData = GetSEData();
            if (string.IsNullOrWhiteSpace(seData.Name))
                return "None";

            return $"{seData.Name} ({seData.Volume}, {seData.Pitch}, {seData.Pan})";
        }
    }
}