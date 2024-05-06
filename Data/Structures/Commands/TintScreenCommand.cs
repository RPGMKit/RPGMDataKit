using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;
using MVTone = RPGMakerLibCoreV2.Data.MV.Classes.Tone;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class TintScreenCommand : NamedCommand
    {
        public override int Code => CommandCodes.TintScreen;
        public override string GetCommandName(StorageVersion version) => "Tint Screen";

        public ITone GetColor(StorageVersion version)
        {
            if (version == StorageVersion.MV)
                return MVTone.FromArray(GetParameter<List<int>>(0));
                
            return GetParameter<ITone>(0);
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
            var color = GetColor(storage.Version);
            var str = $"({color.Red},{color.Green},{color.Blue},{color.Gray}), {GetDuration()} frame(s)";
            if (GetWaitForCompletion())
                str += " (Wait)";

            return str;
        }
    }
}
