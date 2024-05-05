using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfEnemyCommand : IfCommand
    {
        protected const int CHECK_APPEARED = 0;
        protected const int CHECK_STATE = 1;

        public int GetEnemyId()
        {
            return GetParameter<int>(1);
        }

        public int GetCheckType()
        {
            return GetParameter<int>(2);
        }

        public T? GetCheckParameter<T>()
        {
            return GetParameter<T>(3);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"Enemy #{GetEnemyId()}";

            switch (GetCheckType())
            {
                case CHECK_APPEARED:
                    str += " is appeared";
                    break;
                case CHECK_STATE:
                    str += $" is affected by #{GetCheckParameter<int>()} state";
                    break;
                default:
                    throw new Exception($"Unknown check type '{GetCheckType()}' for '{GetCommandName(storage.Version)}' enemy command");
            }

            return str;
        }
    }
}
