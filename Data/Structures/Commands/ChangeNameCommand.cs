using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeNameCommand : NamedCommand
    {
        public override int Code => CommandCodes.ChangeName;
        public override string Name => "Change Name";

        public int GetActor()
        {
            return (int)GetParameter(0);
        }

        public string GetName()
        {
            return (string)GetParameter(1);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"#{GetActor()}, {GetName()}";

            return str;
        }
    }
}