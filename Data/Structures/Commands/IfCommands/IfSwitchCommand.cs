using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.IfCommands;

public class IfSwitchCommand : IfCommand
{
    [EditorDefault(1)]
    public int SwitchId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public OnOffState State
    {
        get => Get<OnOffState>(2);
        set => Set(2, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatSwitch(SwitchId);

        if (context.Version == StorageVersion.MV)
            str += " is ";
        else
            str += " == ";

        str += State switch
        {
            OnOffState.On => "ON",
            OnOffState.Off => "OFF",
            _ => throw new Exception($"Unknown ON/OFF state for '{GetCommandName(context.Version)}' switch command")
        };

        return str;
    }
}