using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeEnemyHpCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeEnemyHp;
    public override string GetCommandName(StorageVersion version) => "Change Enemy HP";
        
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
        
    public int TroopId { get; set; }
    public OperatorType Operator { get; set; }
    public OperandType Operand { get; set; }
    public int Value { get; set; }
    public bool AllowKnockout { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        TroopId = parameterProvider.GetParameter<int>(0);
        Operator = (OperatorType)parameterProvider.GetParameter<int>(1);
        Operand = (OperandType)parameterProvider.GetParameter<int>(2);
        Value = parameterProvider.GetParameter<int>(3);
        AllowKnockout = parameterProvider.GetParameter<bool>(4);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, TroopId);
        parameterProvider.SetParameter(1, (int)Operator);
        parameterProvider.SetParameter(2, (int)Operand);
        parameterProvider.SetParameter(3, Value);
        parameterProvider.SetParameter(4, AllowKnockout);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatTroopMember(TroopId);

        str += Operator switch
        {
            OperatorType.Increase => ", + ",
            OperatorType.Decrease => ", - ",
            _ => throw new Exception($"Unknown modify enemy hp operator '{Operator}'")
        };

        str += Operand switch
        {
            OperandType.Constant => Value,
            OperandType.Variable => context.NameFormatter.FormatVariable(Value),
            _ => throw new Exception($"Unknown modify enemy hp operand '{Operand}'")
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