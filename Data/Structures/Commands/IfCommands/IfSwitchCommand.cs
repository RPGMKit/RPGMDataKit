using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

public class IfSwitchCommand : IfCommand
{
    public int SwitchId { get; set; }   
    public OnOffState State { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Unpack(parameterProvider, version);

        SwitchId = parameterProvider.GetParameter<int>(1);
        State = parameterProvider.GetParameter<OnOffState>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Pack(parameterProvider, version);
        
        parameterProvider.SetParameter(1, SwitchId);
        parameterProvider.SetParameter(2, State);
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