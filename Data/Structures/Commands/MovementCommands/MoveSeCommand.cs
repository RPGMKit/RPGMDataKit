using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveSeCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Se;
        public override string Name => "SE";

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
