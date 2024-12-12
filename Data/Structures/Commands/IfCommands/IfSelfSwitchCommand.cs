using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

public class IfSelfSwitchCommand : IfCommand
{
    public string Switch { get; set; }
    
    public OnOffState State {get; set;}

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Unpack(parameterProvider, version);

        Switch = parameterProvider.GetParameter<string>(1);
        State = (OnOffState)parameterProvider.GetParameter<int>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Pack(parameterProvider, version);
        
        parameterProvider.SetParameter(1, Switch);
        parameterProvider.SetParameter(2, (int)State);
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