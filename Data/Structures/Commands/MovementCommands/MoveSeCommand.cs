using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveSeCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Se;
        public override string GetCommandName(StorageVersion version) => "SE";

        public IAudioFile GetSEData(StorageVersion version)
        {
            return version == StorageVersion.MV ? GetParameter<SoundData>(0) : GetParameter<IAudioFile>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var seData = GetSEData(storage.Version);
            if (string.IsNullOrWhiteSpace(seData.Name))
                return "None";

            var str = $"{seData.Name} ({seData.Volume}, {seData.Pitch}";
            if (storage.Version == StorageVersion.MV)
                str += $", {seData.Pan}";
            str += ")";

            return str;
        }
    }
}
