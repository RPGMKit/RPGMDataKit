using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShowAnimationCommand : NamedCommand
    {
        protected const int ACTOR_PLAYER = -1;
        protected const int ACTOR_THIS_EVENT = 0;

        public override int Code => CommandCodes.ShowAnimation;
        public override string GetCommandName(StorageVersion version) => "Show Animation";

        public int GetActor()
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
            string str;
            switch (GetActor())
            {
                case ACTOR_PLAYER:
                    str = "Player";
                    break;
                case ACTOR_THIS_EVENT:
                    str = "This Event";
                    break;
                default:
                    str = $"Event #{GetActor()}";
                    if (context.Version != StorageVersion.MV)
                        str = $"[{str}]";
                    break;
            }

            str += ", " + context.NameFormatter.FormatAnimation(GetAnimation());

            if (GetWaitForCompletion())
                str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

            return str;
        }
    }
}
