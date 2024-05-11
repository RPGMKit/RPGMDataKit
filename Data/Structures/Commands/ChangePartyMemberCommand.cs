using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangePartyMemberCommand : NamedCommand
    {
        public enum ChangeType
        {
            Add = 0,
            Remove = 1,
        }

        public override int Code => CommandCodes.ChangePartyMember;
        public override string GetCommandName(StorageVersion version) => "Change Party Member";

        public int GetActor()
        {
            return GetParameter<int>(0);
        }

        public ChangeType GetChangeType()
        {
            return (ChangeType)GetParameter<int>(1);
        }

        public bool GetInitialize(StorageVersion version)
        {
            return version == StorageVersion.MV ? GetParameter<bool>(2) : GetParameter<int>(2) > 0;
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = "";
            switch (GetChangeType())
            {
                case ChangeType.Add:
                    str = "Add";
                    break;
                case ChangeType.Remove:
                    str = "Remove";
                    break;
                default:
                    throw new Exception($"Unknown add/remove state for party member");
            }

            str += context.NameFormatter.FormatActor(GetActor());
            if (GetInitialize(context.Version))
                str += context.Version == StorageVersion.MV ? " (Initialize)" : ", Initialize";

            return str;
        }
    }
}
