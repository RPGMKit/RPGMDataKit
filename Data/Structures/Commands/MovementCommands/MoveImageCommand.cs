using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveImageCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Image;

        public override string GetCommandName(StorageVersion version)
        {
            if (version == StorageVersion.MV)
                return "Image";
            else
                return "Graphic";
        }

        public string GetImage()
        {
            return GetParameter<string>(0);
        }

        public int GetOffset()
        {
            return GetParameter<int>(1);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var image = GetImage();
            if (string.IsNullOrWhiteSpace(image))
                return context.Version == StorageVersion.MV ? "None" : "(None)";

            if (context.Version == StorageVersion.MV)
                return $"{image}({GetOffset()})";
            else
                return $"'{image}', {GetOffset()}";
        }
    }
}
