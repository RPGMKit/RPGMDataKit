using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.VXAce.Classes.Custom;
using MVColor = RPGMakerLibCoreV2.Data.MV.Classes.Color;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class FlashScreenCommand : NamedCommand
{
    public override int Code => CommandCodes.FlashScreen;
    public override string GetCommandName(StorageVersion version) => "Flash Screen";

    public IColor Color { get; set; }
    public int Duration { get; set; }
    public bool WaitForCompletion { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        if (version == StorageVersion.MV)
            Color = MVColor.FromArray(parameterProvider.GetParameter<List<int>>(0));
        else
            Color = parameterProvider.GetParameter<IColor>(0);

        Duration = parameterProvider.GetParameter<int>(1);
        WaitForCompletion = parameterProvider.GetParameter<bool>(2);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        if (version == StorageVersion.MV)
            parameterProvider.SetParameter(0, new List<int>() { Color.Red, Color.Green, Color.Blue, Color.Alpha });
        else
            parameterProvider.SetParameter(0,
                new Color() { Red = Color.Red, Green = Color.Green, Blue = Color.Blue, Alpha = Color.Alpha });
        
        parameterProvider.SetParameter(1, Duration);
        parameterProvider.SetParameter(2, WaitForCompletion);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var fmt = context.Version == StorageVersion.MV
            ? "({0},{1},{2},{3}), {4} frame(s)"
            : "({0},{1},{2},{3}), @{4}";

        var str = string.Format(fmt, Color.Red, Color.Green, Color.Blue, Color.Alpha, Duration);
        if (WaitForCompletion)
            str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

        return str;
    }
}