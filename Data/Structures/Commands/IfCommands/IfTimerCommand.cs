using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.IfCommands;

public class IfTimerCommand : IfCommand
{
    public enum OperatorType
    {
        Gte = 0,
        Lte = 1,
    }

    public int Seconds
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public OperatorType Operator
    {
        get => Get<OperatorType>(2);
        set => Set(2, value);
    }

    private static string FormatTime(int seconds)
    {
        var minutes = seconds / 60;
        seconds %= 60;

        return $"{minutes} min {seconds} sec";
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        string fmt;
        if (context.Version == StorageVersion.MV)
            fmt = "Timer {0} {1}";
        else
            fmt = "Timer {1} or {0}";

        var op = Operator switch
        {
            OperatorType.Gte => context.Version == StorageVersion.MV ? "≥" : "more",
            OperatorType.Lte => context.Version == StorageVersion.MV ? "≤" : "less",
            _ => throw new Exception(
                $"Unknown operator '{Operator}' for '{GetCommandName(context.Version)}' timer command")
        };

        return string.Format(fmt, op, FormatTime(Seconds));
    }
}
