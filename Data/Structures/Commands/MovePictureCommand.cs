using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class MovePictureCommand : NamedCommand
    {
        protected const int REFERENCE_UPPERLEFT = 0;
        protected const int REFERENCE_CENTER = 1;
        protected const int PLACE_DIRECT = 0;
        protected const int PLACE_BY_VARIABLE = 1;
        protected const int BLEND_NORMAL = 0;
        protected const int BLEND_ADDITIVE = 1;

        public int GetLayer()
        {
            return (int)GetParameter(0);
        }

        public int GetReference()
        {
            return (int)GetParameter(2);
        }

        public int GetPlaceType()
        {
            return (int)GetParameter(3);
        }

        public int GetX()
        {
            return (int)GetParameter(4);
        }

        public int GetY()
        {
            return (int)GetParameter(5);
        }

        public int GetWidth()
        {
            return (int)GetParameter(6);
        }

        public int GetHeight()
        {
            return (int)GetParameter(7);
        }

        public int GetTransparency()
        {
            return (int)GetParameter(8);
        }

        public int GetBlendMode()
        {
            return (int)GetParameter(9);
        }

        public int GetFrames()
        {
            return (int)GetParameter(10);
        }

        public bool GetWaitCompletion()
        {
            return (bool)GetParameter(11);
        }

        public override int Code => CommandCodes.MovePicture;
        public override string Name => "Move picture";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"#{GetLayer()}, ";
            switch (GetReference())
            {
                case REFERENCE_UPPERLEFT:
                    str += "Upper left";
                    break;
                case REFERENCE_CENTER:
                    str += "Center";
                    break;
                default:
                    throw new Exception($"Unknown show picture reference type '{GetReference()}'");
            }

            switch (GetPlaceType())
            {
                case PLACE_DIRECT:
                    str += $"({GetX()},{GetY()})";
                    break;
                case PLACE_BY_VARIABLE:
                    str += "({#" + GetX();
                    var variableName = storage.GetVariable(GetX());
                    if (!string.IsNullOrEmpty(variableName))
                        str += " " + variableName;

                    str += "},";
                    str += "{#" + GetY();
                    variableName = storage.GetVariable(GetY());
                    if (!string.IsNullOrEmpty(variableName))
                        str += " " + variableName;

                    str += "})";
                    break;
                default:
                    throw new Exception($"Unknown show picture place by type '{GetPlaceType()}'");
            }

            str += $" ({GetWidth()}%,{GetHeight()}%), {GetTransparency()}, ";
            switch (GetBlendMode())
            {
                case BLEND_NORMAL:
                    str += "Normal";
                    break;
                case BLEND_ADDITIVE:
                    str += "Additive";
                    break;
                default:
                    throw new Exception($"Unknown show picture blend mode '{GetBlendMode()}'");
            }

            str += $", {GetFrames()} frames";
            if (GetWaitCompletion())
                str += " (Wait)";

            return str;
        }
    }
}
