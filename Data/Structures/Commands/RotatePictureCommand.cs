using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class RotatePictureCommand : NamedCommand
    {
        public override int Code => CommandCodes.RotatePicture;
        public override string GetCommandName(StorageVersion version) => "Rotate Picture";

        public int GetLayer()
        {
            return GetParameter<int>(0);
        }

        public int GetSpeed()
        {
            return GetParameter<int>(1);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            if (context.Version == StorageVersion.MV)
                return $"#{GetLayer()}, {GetSpeed()}";
            else
            {
                var speed = GetSpeed();
                if (speed >= 0)
                    return $"{GetLayer()}, +{speed}";
                else
                    return $"{GetLayer()}, {speed}";
            }
        }
    }
}
