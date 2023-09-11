using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangePartyMemberCommand : NamedCommand
    {
        protected const int ACTOR_ADD = 0;
        protected const int ACTOR_REMOVE = 1;

        public override int Code => CommandCodes.ChangePartyMember;
        public override string Name => "Change Party Member";

        public int GetActor()
        {
            return (int)GetParameter(0);
        }

        public int GetAddOrRemove()
        {
            return (int)GetParameter(1);
        }

        public bool GetInitialize()
        {
            return (bool)GetParameter(2);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = "";
            switch (GetAddOrRemove())
            {
                case ACTOR_ADD:
                    str = "Add";
                    break;
                case ACTOR_REMOVE:
                    str = "Remove";
                    break;
                default:
                    throw new Exception($"Unknown add/remove state for party member");
            }

            str += $" Actor #{GetActor()}";
            if (GetInitialize())
            {
                str += " (Initialize)";
            }

            return str;
        }
    }
}
