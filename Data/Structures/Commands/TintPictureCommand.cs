using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using MVColor = RPGMakerLibCoreV2.Data.MV.Classes.Color;

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

        public IColor GetColor(StorageVersion version)
        {
            if (version == StorageVersion.MV)
                return MVColor.FromArray(GetParameter<List<int>>(1));

            return GetParameter<IColor>(1);
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
            var str = $"#{GetLayer()}, ({color.Red},{color.Green},{color.Blue},{color.Alpha}), {GetDuration()} frame(s)";
            if (GetWaitForCompletion())
                str += " (Wait)";

            return str;
        }
    }
}
