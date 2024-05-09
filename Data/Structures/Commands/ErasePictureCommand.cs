using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ErasePictureCommand : NamedCommand
    {
        public override int Code => CommandCodes.ErasePicture;
        public override string GetCommandName(StorageVersion version) => "Erase Picture";

        public int GetLayer()
        {
            return GetParameter<int>(0);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return context.Version == StorageVersion.MV ? $"#{GetLayer()}" : GetLayer().ToString();
        }
    }
}
