using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class TintPictureCommand : NamedCommand
    {
        public override int Code => CommandCodes.TintPicture;
        public override string Name => "Tint Picture";

        public int GetLayer()
        {
            return (int)GetParameter(0);
        }
        
        public List<int> GetColor()
        {
            return GetParameter(1)?.ToObject<List<int>>();
        }

        public int GetDuration()
        {
            return (int)GetParameter(2);
        }

        public bool GetWaitForCompletion()
        {
            return (bool)GetParameter(3);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var color = GetColor();
            var str = $"#{GetLayer()}, ({color[0]},{color[1]},{color[2]},{color[3]}), {GetDuration()} frame(s)";
            if (GetWaitForCompletion())
                str += " (Wait)";

            return str;
        }
    }
}
