using RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Versions.VXAce.MovementCommands
{
    public class VXAceMoveSeCommand : MoveSeCommand
    {
        public override IAudioFile GetSEData()
        {
            return GetParameter<IAudioFile>(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var seData = GetSEData();
            if (string.IsNullOrWhiteSpace(seData.Name))
                return "None";

            return $"{seData.Name} ({seData.Volume}, {seData.Pitch})";
        }
    }
}
