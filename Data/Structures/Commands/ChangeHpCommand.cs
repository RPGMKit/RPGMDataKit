using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeHpCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeHp;
    public override string GetCommandName(StorageVersion version) => "Change HP";
        
    public enum SourceType
    {
        Actor = 0,
        Variable = 1,
    }

    public enum OperatorType
    {
        Increase = 0,
        Decrease = 1,
    }

    public enum OperandType
    {
        Constant = 0,
        Variable = 1,
    }

    public SourceType SourceT { get; set; }
    public int SourceId { get; set; }
    public OperatorType Operator { get; set; }
    public OperandType Operand { get; set; }
    public int Value { get; set; }
    public bool AllowKnockout { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        SourceT = parameterProvider.GetParameter<SourceType>(0);
        SourceId = parameterProvider.GetParameter<int>(1);
        Operator = parameterProvider.GetParameter<OperatorType>(2);
        Operand = parameterProvider.GetParameter<OperandType>(3);
        Value = parameterProvider.GetParameter<int>(4);
        AllowKnockout = parameterProvider.GetParameter<bool>(5);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, SourceT);
        parameterProvider.SetParameter(1, SourceId);
        parameterProvider.SetParameter(2, Operator);
        parameterProvider.SetParameter(3, Operand);
        parameterProvider.SetParameter(4, Value);
        parameterProvider.SetParameter(5, AllowKnockout);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = SourceT switch
        {
            SourceType.Actor => context.NameFormatter.FormatActor(SourceId),
            SourceType.Variable => context.NameFormatter.FormatVariable(SourceId),
            _ => throw new Exception($"Unknown modify hp source type '{SourceT}'")
        };

        str += Operator switch
        {
            OperatorType.Increase => ", + ",
            OperatorType.Decrease => ", - ",
            _ => throw new Exception($"Unknown modify hp operator '{Operator}'")
        };

        str += Operand switch
        {
            OperandType.Constant => Value,
            OperandType.Variable => context.NameFormatter.FormatVariable(Value),
            _ => throw new Exception($"Unknown modify hp operand '{Operand}'")
        };

        if (AllowKnockout)
        {
            if (context.Version == StorageVersion.MV)
                str += " (Allow Knockout)";
            else
                // custom
                str += ", Allow Knockout";
        }

        return str;
    }
}