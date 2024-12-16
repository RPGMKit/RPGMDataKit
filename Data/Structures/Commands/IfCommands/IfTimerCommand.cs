using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

public class IfTimerCommand : IfCommand
{
    public enum OperatorType
    {
        Gte = 0,
        Lte = 1,
    }

    public int Seconds { get; set; }
    public OperatorType Operator { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Unpack(parameterProvider, version);

        Seconds = parameterProvider.GetParameter<int>(1);
        Operator = parameterProvider.GetParameter<OperatorType>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Pack(parameterProvider, version);
        
        parameterProvider.SetParameter(1, Seconds);
        parameterProvider.SetParameter(2, Operator);
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
