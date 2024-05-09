using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class PlayMECommand : NamedCommand
    {
        public override int Code => CommandCodes.PlayMe;
        public override string GetCommandName(StorageVersion version) => "Play ME";

        public IAudioFile GetSEData(StorageVersion version)
        {
            return version == StorageVersion.MV ? GetParameter<SoundData>(0) : GetParameter<IAudioFile>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var meData = GetSEData(context.Version);
            
            if (context.Version == StorageVersion.MV)
            {
                if (string.IsNullOrWhiteSpace(meData.Name))
                    return "None";

                return $"{meData.Name} ({meData.Volume}, {meData.Pitch}, {meData.Pan})";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(meData.Name))
                    return "(None)";

                return $"'{meData.Name}', {meData.Volume}, {meData.Pitch}";
            }
        }
    }
}