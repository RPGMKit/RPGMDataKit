using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveImageCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Image;
        public override string GetCommandName(StorageVersion version) => "Image";

        public string GetImage()
        {
            return GetParameter<string>(0);
        }

        public int GetOffset()
        {
            return GetParameter<int>(1);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var image = GetImage();
            if (string.IsNullOrWhiteSpace(image))
                return "None";
            else
                return $"{image}({GetOffset()})";
        }
    }
}
