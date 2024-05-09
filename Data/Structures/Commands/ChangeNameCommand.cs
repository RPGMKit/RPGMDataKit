using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeNameCommand : NamedCommand
    {
        public override int Code => CommandCodes.ChangeName;
        public override string GetCommandName(StorageVersion version) => "Change Name";

        public int GetActor()
        {
            return GetParameter<int>(0);
        }

        public string GetName()
        {
            return GetParameter<string>(1);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatActor(GetActor());
            if (context.Version == StorageVersion.MV)
                str += $", {GetName()}";
            else
                str += $", '{GetName()}'";

            return str;
        }
    }
}