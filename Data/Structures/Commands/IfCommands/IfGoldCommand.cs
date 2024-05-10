using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

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
        string fmt;
        switch (GetOperator())
        {
            case OPERATOR_GTE:
                fmt = context.Version == StorageVersion.MV ? "≥ {0}" : "{0} or more";
                break;
            case OPERATOR_LTE:
                fmt = context.Version == StorageVersion.MV ? "≤ {0}" : "{0} or less";
                break;
            case OPERATOR_LT:
                fmt = context.Version == StorageVersion.MV ? "< {0}" : "is under {0}";
                break;
            default:
                throw new Exception(
                    $"Unknown operator '{GetOperator()}' for '{GetCommandName(context.Version)}' gold command");
        }

        return "Gold " + string.Format(fmt, GetAmount());
    }
}
