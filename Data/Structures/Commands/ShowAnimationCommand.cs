using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShowAnimationCommand : NamedCommand
    {
        public override int Code => CommandCodes.ShowAnimation;
        public override string GetCommandName(StorageVersion version) => "Show Animation";

        public int GetEvent()
        {
            return GetParameter<int>(0);
        }

        public int GetAnimation()
        {
            return GetParameter<int>(1);
        }

        public bool GetWaitForCompletion()
        {
            return GetParameter<bool>(2);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatEvent(GetEvent());

            str += ", " + context.NameFormatter.FormatAnimation(GetAnimation());

            if (GetWaitForCompletion())
                str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

            return str;
        }
    }
}
