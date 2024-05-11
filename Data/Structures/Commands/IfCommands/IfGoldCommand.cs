using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

public class IfGoldCommand : IfCommand
{
    public enum OperatorType
    {
        Gte = 0,
        Lte = 1,
        Lt = 2,
    }

    public int GetAmount()
    {
        return GetParameter<int>(1);
    }

    public OperatorType GetOperator()
    {
        return (OperatorType)GetParameter<int>(2);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        string fmt;
        switch (GetOperator())
        {
            case OperatorType.Gte:
                fmt = context.Version == StorageVersion.MV ? "≥ {0}" : "{0} or more";
                break;
            case OperatorType.Lte:
                fmt = context.Version == StorageVersion.MV ? "≤ {0}" : "{0} or less";
                break;
            case OperatorType.Lt:
                fmt = context.Version == StorageVersion.MV ? "< {0}" : "is under {0}";
                break;
            default:
                throw new Exception(
                    $"Unknown operator '{GetOperator()}' for '{GetCommandName(context.Version)}' gold command");
        }

        return "Gold " + string.Format(fmt, GetAmount());
    }
}
