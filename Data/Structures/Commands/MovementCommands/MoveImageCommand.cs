using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public class MoveImageCommand : AbstractMoveCommand
    {
        public override int Code => MovementRouteCommandCodes.Image;
        public override string Name => "Image";

        public string GetImage()
        {
            return (string)GetParameter(0);
        }

        public int GetOffset()
        {
            return (int)GetParameter(1);
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
