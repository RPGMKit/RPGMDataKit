using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShowScrollingTextBaseCommand : NamedCommand
    {
        public override int Code => CommandCodes.ShowScrollingTextBase;
        public override string GetCommandName(StorageVersion version) => "Text(S)";

        public int GetSpeed()
        {
            return GetParameter<int>(0);
        }

        public bool GetNoFastForward()
        {
            return GetParameter<bool>(1);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = $"Speed {GetSpeed()}";
            if (GetNoFastForward())
                str += ", No Fast Forward";

            return str;
        }
    }
}
