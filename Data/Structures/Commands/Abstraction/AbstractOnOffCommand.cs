using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

public abstract class AbstractOnOffCommand : NamedCommand
{
    public OnOffState State { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        State = parameterProvider.GetParameter<OnOffState>(0);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, State);
    }
    
    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return State switch
        {
            OnOffState.On => "ON",
            OnOffState.Off => "OFF",
            _ => throw new Exception($"Unknown state {State} for {GetCommandName(context.Version)} command")
        };
    }
}