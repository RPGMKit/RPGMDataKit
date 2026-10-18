using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ControlSelfSwitchCommand : NamedCommand
{
    public override int Code => CommandCodes.ControlSelfSwitch;
    public override string GetCommandName(StorageVersion version) => "Control Self Switch";
        
    [EditorDefault("A")]
    public string Switch
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    public OnOffState State
    {
        get => Get<OnOffState>(1);
        set => Set(1, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = Switch + State switch
        {
            OnOffState.On => " = ON",
            OnOffState.Off => " = OFF",
            _ => throw new Exception(
                $"Unknown on/off state {State} for action #{Code} {nameof(ControlSelfSwitchCommand)}")
        };

        return str;
    }
}