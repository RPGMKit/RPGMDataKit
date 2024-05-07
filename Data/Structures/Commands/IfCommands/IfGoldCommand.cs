using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfGoldCommand : IfCommand
    {
        protected const int OPERATOR_GTE = 0;
        protected const int OPERATOR_LTE = 1;
        protected const int OPERATOR_LT = 2;

        public int GetAmount()
        {
            return GetParameter<int>(1);
        }

        public int GetOperator()
        {
            return GetParameter<int>(2);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = "Gold ";

            switch (GetOperator())
            {
                case OPERATOR_GTE:
                    str += ">=";
                    break;
                case OPERATOR_LTE:
                    str += "<=";
                    break;
                case OPERATOR_LT:
                    str += "<";
                    break;
                default:
                    throw new Exception($"Unknown operator '{GetOperator()}' for '{GetCommandName(context.Version)}' gold command");
            }

            str += $" {GetAmount()}";

            return str;
        }
    }
}
