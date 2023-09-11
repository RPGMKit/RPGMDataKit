namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfTimerCommand : IfCommand
    {
        protected const int OPERATOR_GTE = 0;
        protected const int OPERATOR_LTE = 1;

        public int GetSeconds()
        {
            return (int)GetParameter(1);
        }

        public int GetOperator()
        {
            return (int)GetParameter(2);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = "Timer ";

            switch (GetOperator())
            {
                case OPERATOR_GTE:
                    str += ">=";
                    break;
                case OPERATOR_LTE:
                    str += "<=";
                    break;
                default:
                    throw new Exception($"Unknown operator '{GetOperator()}' for '{Name}' timer command");
            }

            str += $"{GetSeconds()} sec";

            return str;
        }
    }
}
