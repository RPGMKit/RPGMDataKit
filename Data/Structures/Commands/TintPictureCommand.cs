using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class TintPictureCommand : NamedCommand
    {
        public override int Code => CommandCodes.TintPicture;
        public override string GetCommandName(StorageVersion version) => "Tint Picture";

        public int GetLayer()
        {
            return GetParameter<int>(0);
        }

        public ITone GetColor(StorageVersion version)
        {
            if (version == StorageVersion.MV)
                return Tone.FromArray(GetParameter<List<int>>(1));

            return GetParameter<ITone>(1);
        }

        public int GetDuration()
        {
            return GetParameter<int>(2);
        }

        public bool GetWaitForCompletion()
        {
            return GetParameter<bool>(3);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var color = GetColor(context.Version);
            
            var fmt = context.Version == StorageVersion.MV
                ? "#{0}, ({1},{2},{3},{4}), {5} frame(s)"
                : "{0}, ({1},{2},{3},{4}), @{5}";

            var str = string.Format(fmt, GetLayer(), color.Red, color.Green, color.Blue, color.Gray, GetDuration());
            if (GetWaitForCompletion())
                str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

            return str;
        }
    }
}
