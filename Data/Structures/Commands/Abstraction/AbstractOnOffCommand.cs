using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

public abstract class AbstractOnOffCommand : NamedCommand
{
    public OnOffState GetState() => (OnOffState)GetParameter<int>(0);

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return GetState() switch
        {
            OnOffState.On => "ON",
            OnOffState.Off => "OFF",
            _ => throw new Exception($"Unknown state {GetState()} for {GetCommandName(context.Version)} command")
        };
    }
}