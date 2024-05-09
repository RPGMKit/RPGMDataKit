using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
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

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var color = GetColor(context.Version);
            var fmt = context.Version == StorageVersion.MV
                ? "#({0},{1},{2},{3}), {4} frame(s)"
                : "({0},{1},{2},{3}), @{4}";

            var str = string.Format(fmt, color.Red, color.Green, color.Blue, color.Gray, GetDuration());
            if (GetWaitForCompletion())
                str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

            return str;
        }
    }
}
