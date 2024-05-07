using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShowTextBaseCommand : NamedCommand
    {
        protected const int BACKGROUND_WINDOW = 0;
        protected const int BACKGROUND_DIM = 1;
        protected const int BACKGROUND_TRANSPARENT = 2;
        protected const int POSITION_TOP = 0;
        protected const int POSITION_MIDDLE = 1;
        protected const int POSITION_BOTTOM = 2;

        public override int Code => CommandCodes.ShowTextBase;
        public override string GetCommandName(StorageVersion version) => "Text";

        public string GetFace()
        {
            return GetParameter<string>(0);
        }

        public int GetOffset()
        {
            return GetParameter<int>(1);
        }

        public int GetBackground()
        {
            return GetParameter<int>(2);
        }

        public int GetPosition()
        {
            return GetParameter<int>(3);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var face = GetFace();
            if (string.IsNullOrWhiteSpace(face))
                face = context.Version == StorageVersion.VXAce ? "-, -" : "None";
            else
                face = context.Version == StorageVersion.MV ? $"{face}({GetOffset()})" : $"'{face}', {GetOffset()}";

            var background = "";
            switch (GetBackground())
            {
                case BACKGROUND_WINDOW:
                    background = context.Version == StorageVersion.VXAce ? "Normal" : "Window";
                    break;
                case BACKGROUND_DIM:
                    background = context.Version == StorageVersion.VXAce ? "Dark" : "Dim";
                    break;
                case BACKGROUND_TRANSPARENT:
                    background = "Transparent";
                    break;
                default:
                    throw new Exception($"Unknown background '{GetBackground()}'");
            }

            var position = "";
            switch (GetPosition())
            {
                case POSITION_TOP:
                    position = "Top";
                    break;
                case POSITION_MIDDLE:
                    position = "Middle";
                    break;
                case POSITION_BOTTOM:
                    position = "Bottom";
                    break;
                default:
                    throw new Exception($"Unknown position '{GetPosition()}'");
            }

            return $"{face}, {background}, {position}";
        }
    }
}
