using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

public class IfTimerCommand : IfCommand
{
    protected const int OPERATOR_GTE = 0;
    protected const int OPERATOR_LTE = 1;

    public int GetSeconds()
    {
        return GetParameter<int>(1);
    }

    public int GetOperator()
    {
        return GetParameter<int>(2);
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

        string op = "";

        switch (GetOperator())
        {
            case OPERATOR_GTE:
                op = context.Version == StorageVersion.MV ? "≥" : "more";
                break;
            case OPERATOR_LTE:
                op = context.Version == StorageVersion.MV ? "≤" : "less";
                break;
            default:
                throw new Exception(
                    $"Unknown operator '{GetOperator()}' for '{GetCommandName(context.Version)}' timer command");
        }

        return string.Format(fmt, op, FormatTime(GetSeconds()));
    }
}
