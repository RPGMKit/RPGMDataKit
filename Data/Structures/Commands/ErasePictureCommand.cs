using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ErasePictureCommand : NamedCommand
    {
        public override int Code => CommandCodes.ErasePicture;
        public override string Name => "Erase Picture";

        public int GetLayer()
        {
            return (int)GetParameter(0);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return $"#{GetLayer()}";
        }
    }
}
