using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class CallCommonEventCommand : NamedCommand
{
    public override int Code => CommandCodes.CallCommonEvent;
    public override string GetCommandName(StorageVersion version)
    {
        return version == StorageVersion.MV ? "Common Event" : "Call Common Event";
    }
        
    [EditorDefault(1, StorageVersion.VXAce)]
    public int EventId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatCommonEvent(EventId);
    }
}