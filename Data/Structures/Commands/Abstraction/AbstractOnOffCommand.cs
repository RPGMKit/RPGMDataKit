using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.Abstraction;

public abstract class AbstractOnOffCommand : NamedCommand
{
    public OnOffState State
    {
        get => Get<OnOffState>(0);
        set => Set(0, value);
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