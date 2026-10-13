using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ControlSwitchCommand : NamedCommand
{
    public override int Code => CommandCodes.ControlSwitches;
    public override string GetCommandName(StorageVersion version) => "Control Switches";
        
    public int StartSwitchId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public int EndSwitchId
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
        string str;
        if (StartSwitchId != EndSwitchId)
            str = context.NameFormatter.FormatSwitchSpan(StartSwitchId, EndSwitchId);
        else
        {
            if (context.Version == StorageVersion.MV)
            {
                str = context.NameFormatter.FormatSwitch(StartSwitchId, new()
                {
                    Separator = " ",
                    Prefix = "#",
                    SkipId = false,
                    SkipTitle = true,
                });
            }
            else
                str = context.NameFormatter.FormatSwitch(StartSwitchId, new() { SkipTitle = true });
        }

        str += State switch
        {
            OnOffState.On => " = ON",
            OnOffState.Off => " = OFF",
            _ => throw new Exception(
                $"Unknown on/off state {State} for action #{Code} {GetCommandName(context.Version)}")
        };

        return str;
    }
}