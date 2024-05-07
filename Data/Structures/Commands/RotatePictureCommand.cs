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
            return $"#{GetLayer()}, {GetSpeed()}";
        }
    }
}
