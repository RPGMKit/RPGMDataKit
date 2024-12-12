using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class CallCommonEventCommand : NamedCommand
{
    public override int Code => CommandCodes.CallCommonEvent;
    public override string GetCommandName(StorageVersion version)
    {
        return version == StorageVersion.MV ? "Common Event" : "Call Common Event";
    }
        
    public int EventId { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        EventId = parameterProvider.GetParameter<int>(0);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, EventId);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatCommonEvent(EventId);
    }
}