using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ShakeScreenCommand : NamedCommand
{
    public override int Code => CommandCodes.ShakeScreen;
    public override string GetCommandName(StorageVersion version) => "Shake Screen";

    public int Power { get; set; }
    public int Speed { get; set; }
    public int Duration { get; set; }
    public bool WaitForCompletion { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Power = parameterProvider.GetParameter<int>(0);
        Speed = parameterProvider.GetParameter<int>(1);
        Duration = parameterProvider.GetParameter<int>(2);
        WaitForCompletion = parameterProvider.GetParameter<bool>(3);
    }
    
    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, Power);
        parameterProvider.SetParameter(1, Speed);
        parameterProvider.SetParameter(2, Duration);
        parameterProvider.SetParameter(3, WaitForCompletion);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        if (context.Version == StorageVersion.MV)
        {
            var str = $"{Power}, {Speed}, {Duration} frame(s)";
            if (WaitForCompletion)
                str += " (Wait)";

            return str;
        }
        else
        {
            var str = $"{Power}, {Speed}, @{Duration}";
            if (WaitForCompletion)
                str += ", Wait";

            return str;
        }
    }
}