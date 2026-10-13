using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ControlTimerCommand : NamedCommand
{
    public override int Code => CommandCodes.ControlTimer;
    public override string GetCommandName(StorageVersion version) => "Control Timer";
        
    public OnOffState State
    {
        get => Get<OnOffState>(0);
        set => Set(0, value);
    }

    public int Seconds
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        if (State == OnOffState.Off)
            return "Stop";

        var sec = Seconds;
        var min = sec / 60;
        sec %= 60;

        if (context.Version == StorageVersion.MV)
            return $"Start, {min} min {sec} sec";
        else
            return $"Startup ({min} min. {sec} sec.)";
    }
}