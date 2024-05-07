using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangePartyMemberCommand : NamedCommand
    {
        protected const int ACTOR_ADD = 0;
        protected const int ACTOR_REMOVE = 1;

        public override int Code => CommandCodes.ChangePartyMember;
        public override string GetCommandName(StorageVersion version) => "Change Party Member";

        public int GetActor()
        {
            return GetParameter<int>(0);
        }

        public int GetAddOrRemove()
        {
            return GetParameter<int>(1);
        }

        public bool GetInitialize(StorageVersion version)
        {
            return version == StorageVersion.MV ? GetParameter<bool>(2) : GetParameter<int>(2) > 0;
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
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
            if (GetInitialize(context.Version))
            {
                str += " (Initialize)";
            }

            return str;
        }
    }
}
