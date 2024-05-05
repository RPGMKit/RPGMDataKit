using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class TintScreenCommand : NamedCommand
    {
        public override int Code => CommandCodes.TintScreen;
        public override string Name => "Tint Screen";

        public List<int> GetColor()
        {
            return GetParameter<List<int>>(0);
        }

        public int GetDuration()
        {
            return GetParameter<int>(1);
        }

        public bool GetWaitForCompletion()
        {
            return GetParameter<bool>(2);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var color = GetColor();
            var str = $"({color[0]},{color[1]},{color[2]},{color[3]}), {GetDuration()} frame(s)";
            if (GetWaitForCompletion())
                str += " (Wait)";

            return str;
        }
    }
}
