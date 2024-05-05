using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeParallaxCommand : NamedCommand
    {
        public override int Code => CommandCodes.ChangeParallax;
        public override string Name => "Change Parallax";

        public string GetImage()
        {
            return GetParameter<string>(0);
        }

        public bool GetLoopHorizontally()
        {
            return GetParameter<bool>(1);
        }

        public bool GetLoopVertically()
        {
            return GetParameter<bool>(2);
        }

        public int GetHorizontalScroll()
        {
            return GetParameter<int>(3);
        }

        public int GetVerticalScroll()
        {
            return GetParameter<int>(4);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = GetImage();

            List<string> p = new();
            if (GetLoopHorizontally())
                p.Add("Loop Horizontally");

            if (GetLoopVertically())
                p.Add("Loop Vertically");

            if (p.Count > 0)
                str += " (" + string.Join(", ", p) + ")";

            return str;
        }
    }
}
