using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.IfCommands;

public class IfSelfSwitchCommand : IfCommand
{
    [EditorDefault("A")]
    public string Switch
    {
        get => Get<string>(1);
        set => Set(1, value);
    }

    public OnOffState State
    {
        get => Get<OnOffState>(2);
        set => Set(2, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = $"Self Switch {Switch}";

        if (context.Version == StorageVersion.MV)
            str += " is ";
        else
            str += " == ";

        str += State switch
        {
            OnOffState.On => "ON",
            OnOffState.Off => "OFF",
            _ => throw new Exception(
                $"Unknown ON/OFF state for '{GetCommandName(context.Version)}' self switch command")
        };

        return str;
    }
}