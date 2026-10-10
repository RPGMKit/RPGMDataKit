using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class CallCommonEventCommand : NamedCommand
{
    public override int Code => CommandCodes.CallCommonEvent;
    public override string GetCommandName(StorageVersion version)
    {
        return version == StorageVersion.MV ? "Common Event" : "Call Common Event";
    }
        
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