using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ControlSelfSwitchCommand : NamedCommand
{
    public override int Code => CommandCodes.ControlSelfSwitch;
    public override string GetCommandName(StorageVersion version) => "Control Self Switch";
        
    public string Switch { get; set; }
    public OnOffState State { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Switch = parameterProvider.GetParameter<string>(0);
        State = parameterProvider.GetParameter<OnOffState>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Switch);
        parameterProvider.SetParameter(1, State);
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