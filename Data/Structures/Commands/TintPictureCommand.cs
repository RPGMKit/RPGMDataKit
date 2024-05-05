using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class TintPictureCommand : NamedCommand
    {
        public override int Code => CommandCodes.TintPicture;
        public override string Name => "Tint Picture";

        public int GetLayer()
        {
            return GetParameter<int>(0);
        }
        
        public List<int> GetColor()
        {
            return GetParameter<List<int>>(1);
        }

        public int GetDuration()
        {
            return GetParameter<int>(2);
        }

        public bool GetWaitForCompletion()
        {
            return GetParameter<bool>(3);
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
