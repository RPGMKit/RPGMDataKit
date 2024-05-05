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
        public override string Name => "Text";

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

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var face = GetFace();
            if (string.IsNullOrWhiteSpace(face))
                face = "None";
            else
                face = $"{face}({GetOffset()})";

            var background = "";
            switch (GetBackground())
            {
                case BACKGROUND_WINDOW:
                    background = "Window";
                    break;
                case BACKGROUND_DIM:
                    background = "Dim";
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
