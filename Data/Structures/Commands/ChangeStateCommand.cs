using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeStateCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeState;
    public override string GetCommandName(StorageVersion version) => "Change State";
        
    public enum SourceType
    {
        Actor = 0,
        Variable = 1,
    }

    public enum OperatorType
    {
        Add = 0,
        Remove = 1,
    }

    public SourceType SourceT { get; set; }
    public int SourceId { get; set; }
    public OperatorType Operator { get; set; }
    public int StateId { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        SourceT = (SourceType)parameterProvider.GetParameter<int>(0);
        SourceId = parameterProvider.GetParameter<int>(1);
        Operator = (OperatorType)parameterProvider.GetParameter<int>(2);
        StateId = parameterProvider.GetParameter<int>(3);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, (int)SourceT);
        parameterProvider.SetParameter(1, SourceId);
        parameterProvider.SetParameter(2, (int)Operator);
        parameterProvider.SetParameter(3, StateId);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = SourceT switch
        {
            SourceType.Actor => context.NameFormatter.FormatActor(SourceId),
            SourceType.Variable => context.NameFormatter.FormatVariable(SourceId),
            _ => throw new Exception($"Unknown change state source type '{SourceT}'")
        };

        str += Operator switch
        {
            OperatorType.Add => ", + ",
            OperatorType.Remove => ", - ",
            _ => throw new Exception($"Unknown modify state operator '{Operator}'")
        };

        str += context.NameFormatter.FormatState(StateId);

        return str;
    }
}