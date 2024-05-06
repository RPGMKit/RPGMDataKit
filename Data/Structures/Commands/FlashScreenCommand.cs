using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;
using MVColor = RPGMakerLibCoreV2.Data.MV.Classes.Color;
using VXAceColor = RPGMakerLibCoreV2.Data.VXAce.Classes.Custom.Color;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class FlashScreenCommand : NamedCommand
    {
        public override int Code => CommandCodes.FlashScreen;
        public override string GetCommandName(StorageVersion version) => "Flash Screen";

        public IColor GetColor(StorageVersion version)
        {
            if (version == StorageVersion.VXAce)
                return GetParameter<VXAceColor>(0);


            return MVColor.FromArray(GetParameter<List<int>>(0));
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
            var str = $"({color.Red},{color.Green},{color.Blue},{color.Alpha}), {GetDuration()} frame(s)";
            if (GetWaitForCompletion())
                str += " (Wait)";

            return str;
        }
    }
}
