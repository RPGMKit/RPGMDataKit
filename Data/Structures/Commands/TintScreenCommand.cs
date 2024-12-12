using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

using VXTone = VXAce.Classes.Custom.Tone;
using MVTone = Tone;

public class TintScreenCommand : NamedCommand
{
    public override int Code => CommandCodes.TintScreen;
    public override string GetCommandName(StorageVersion version) => "Tint Screen";

    public ITone Tone { get; set; }
    public int Duration { get; set; }
    public bool WaitForCompletion { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        if (version == StorageVersion.MV)
            Tone = MVTone.FromArray(parameterProvider.GetParameter<List<int>>(0));
        else
            Tone = parameterProvider.GetParameter<ITone>(0);
        
        Duration = parameterProvider.GetParameter<int>(1);
        WaitForCompletion = parameterProvider.GetParameter<bool>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        if (version == StorageVersion.MV)
            parameterProvider.SetParameter(0, new List<int>{Tone.Red, Tone.Green, Tone.Blue, Tone.Gray});
        else
            parameterProvider.SetParameter(0, new VXTone(){Red = Tone.Red, Green = Tone.Green, Blue = Tone.Blue, Gray = Tone.Gray});
        
        parameterProvider.SetParameter(1, Duration);
        parameterProvider.SetParameter(2, WaitForCompletion);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var fmt = context.Version == StorageVersion.MV
            ? "({0},{1},{2},{3}), {4} frame(s)"
            : "({0},{1},{2},{3}), @{4}";

        var str = string.Format(fmt, Tone.Red, Tone.Green, Tone.Blue, Tone.Gray, Duration);
        if (WaitForCompletion)
            str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

        return str;
    }
}