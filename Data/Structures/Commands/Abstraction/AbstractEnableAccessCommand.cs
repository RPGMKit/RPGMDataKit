using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

public abstract class AbstractEnableAccessCommand : NamedCommand
{
    public DisableEnableState State { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        State = (DisableEnableState)parameterProvider.GetParameter<int>(0);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, (int)State);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return State switch
        {
            DisableEnableState.Disable => "Disable",
            DisableEnableState.Enable => "Enable",
            _ => throw new Exception($"Unknown state {State} for {GetCommandName(context.Version)} command")
        };
    }
}