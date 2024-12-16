using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ControlSwitchCommand : NamedCommand
{
    public override int Code => CommandCodes.ControlSwitches;
    public override string GetCommandName(StorageVersion version) => "Control Switches";
        
    public int StartSwitchId { get; set; }
    public int EndSwitchId { get; set; }
    public OnOffState State { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        StartSwitchId = parameterProvider.GetParameter<int>(0);
        EndSwitchId = parameterProvider.GetParameter<int>(1);
        State = parameterProvider.GetParameter<OnOffState>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, StartSwitchId);
        parameterProvider.SetParameter(1, EndSwitchId);
        parameterProvider.SetParameter(2, State);
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