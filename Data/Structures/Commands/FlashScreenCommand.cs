using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class FlashScreenCommand : NamedCommand
    {
        public override int Code => CommandCodes.FlashScreen;
        public override string GetCommandName(StorageVersion version) => "Flash Screen";

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
