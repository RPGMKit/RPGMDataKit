using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class NameInputProcessingCommand : NamedCommand
{
    public override int Code => CommandCodes.NameInputProcessing;
    public override string GetCommandName(StorageVersion version) => "Name Input Processing";
        
    public int ActorId { get; set; }
    public int MaxCharacters { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        ActorId = parameterProvider.GetParameter<int>(0);
        MaxCharacters = parameterProvider.GetParameter<int>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, ActorId);
        parameterProvider.SetParameter(1, MaxCharacters);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var actorName = context.NameFormatter.FormatActor(ActorId, new()
        {
            BracketType = BracketType.None
        });

        var str = $"{actorName}, {MaxCharacters} character(s)";

        return str;
    }
}