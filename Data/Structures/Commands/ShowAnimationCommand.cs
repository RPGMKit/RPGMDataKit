using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShowAnimationCommand : NamedCommand
    {
        protected const int ACTOR_PLAYER = -1;
        protected const int ACTOR_THIS_EVENT = 0;

        public override int Code => CommandCodes.ShowAnimation;
        public override string Name => "Show Animation";

        public int GetActor()
        {
            return (int)GetParameter(0);
        }

        public int GetAnimation()
        {
            return (int)GetParameter(1);
        }

        public bool GetWaitForCompletion()
        {
            return (bool)GetParameter(2);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = "";
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
                    break;
            }

            var animation = storage.GetAnimation(GetAnimation());
            if (animation != null)
                str += ", " + animation.Name;
            else
                str += $", Animation #{GetAnimation()}";

            if (GetWaitForCompletion())
                str += " (Wait)";

            return str;
        }
    }
}
