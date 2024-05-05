using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeNameCommand : NamedCommand
    {
        public override int Code => CommandCodes.ChangeName;
        public override string Name => "Change Name";

        public int GetActor()
        {
            return GetParameter<int>(0);
        }

        public string GetName()
        {
            return GetParameter<string>(1);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"#{GetActor()}, {GetName()}";

            return str;
        }
    }
}