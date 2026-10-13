using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.IfCommands;

public class IfGoldCommand : IfCommand
{
    public enum OperatorType
    {
        Gte = 0,
        Lte = 1,
        Lt = 2,
    }

    public int Amount
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public OperatorType Operator
    {
        get => Get<OperatorType>(2);
        set => Set(2, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        string fmt = Operator switch
        {
            OperatorType.Gte => context.Version == StorageVersion.MV ? "≥ {0}" : "{0} or more",
            OperatorType.Lte => context.Version == StorageVersion.MV ? "≤ {0}" : "{0} or less",
            OperatorType.Lt => context.Version == StorageVersion.MV ? "< {0}" : "is under {0}",
            _ => throw new Exception(
                $"Unknown operator '{Operator}' for '{GetCommandName(context.Version)}' gold command")
        };

        return "Gold " + string.Format(fmt, Amount);
    }
}
