using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ControlTimerCommand : NamedCommand
{
    public override int Code => CommandCodes.ControlTimer;
    public override string GetCommandName(StorageVersion version) => "Control Timer";
        
    public OnOffState State { get; set; }
    public int Seconds { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        State = parameterProvider.GetParameter<OnOffState>(0);
        Seconds = parameterProvider.GetParameter<int>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, State);
        parameterProvider.SetParameter(1, Seconds);
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