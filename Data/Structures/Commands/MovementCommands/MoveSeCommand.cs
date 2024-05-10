using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

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

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var seData = GetSEData(context.Version);
            if (string.IsNullOrWhiteSpace(seData.Name))
                return context.Version == StorageVersion.MV ? "None" : "(None)";

            if (context.Version == StorageVersion.MV)
                return $"{seData.Name} ({seData.Volume}, {seData.Pitch}, {seData.Pan})";
            else
                return $"'{seData.Name}', {seData.Volume}, {seData.Pitch}";
        }
    }
}
