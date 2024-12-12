using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class RecoverAllCommand : NamedCommand
{
    public override int Code => CommandCodes.RecoverAll;
    public override string GetCommandName(StorageVersion version) => "Recover All";
    
    public enum SourceType
    {
        Actor = 0,
        Variable = 1,
    }

    public SourceType Source { get; set; }
    public int SourceId { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Source = (SourceType)parameterProvider.GetParameter<int>(0);
        SourceId = parameterProvider.GetParameter<int>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, (int)Source);
        parameterProvider.SetParameter(1, SourceId);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = Source switch
        {
            SourceType.Actor => context.NameFormatter.FormatActor(SourceId),
            SourceType.Variable => context.NameFormatter.FormatVariable(SourceId),
            _ => throw new Exception($"Unknown recover all source type '{Source}'")
        };

        return str;
    }
}