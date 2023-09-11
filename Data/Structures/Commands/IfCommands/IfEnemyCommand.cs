namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfEnemyCommand : IfCommand
    {
        protected const int CHECK_APPEARED = 0;
        protected const int CHECK_STATE = 1;

        public int GetEnemyId()
        {
            return (int)GetParameter(1);
        }

        public int GetCheckType()
        {
            return (int)GetParameter(2);
        }

        public object GetCheckParameter()
        {
            return GetParameter(3);
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
                    str += $" is affected by #{GetCheckParameter()} state";
                    break;
                default:
                    throw new Exception($"Unknown check type '{GetCheckType()}' for '{Name}' enemy command");
            }

            return str;
        }
    }
}
