using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
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

    public int Amount { get; set; }
    public OperatorType Operator { get; set; }
    
    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Unpack(parameterProvider, version);

        Amount = parameterProvider.GetParameter<int>(1);
        Operator = parameterProvider.GetParameter<OperatorType>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Pack(parameterProvider, version);
        
        parameterProvider.SetParameter(1, Amount);
        parameterProvider.SetParameter(2, Operator);
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
