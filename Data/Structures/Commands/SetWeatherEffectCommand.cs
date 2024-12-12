using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class SetWeatherEffectCommand : NamedCommand
{
    public override int Code => CommandCodes.SetWeatherEffect;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Set Weather Effect";
        else
            return "Set Weather Effects";
    }
    
    protected const string TypeNone = "none";

    public string EffectType { get; set; }
    public int Power { get; set; }
    public int Duration { get; set; }
    public bool WaitForCompletion { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        EffectType = parameterProvider.GetParameter<string>(0);
        Power = parameterProvider.GetParameter<int>(1);
        Duration = parameterProvider.GetParameter<int>(2);  
        WaitForCompletion = parameterProvider.GetParameter<bool>(3);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, EffectType);
        parameterProvider.SetParameter(1, Power);
        parameterProvider.SetParameter(2, Duration);
        parameterProvider.SetParameter(3, WaitForCompletion);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = "";
        if (EffectType == TypeNone)
            str = "None";
        else
            str = EffectType.UcFirst() + $", {Power}";

        str += context.Version == StorageVersion.MV ? $", {Duration} frame(s)" : $", @{Duration}";
        if (WaitForCompletion)
            str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

        return str;
    }
}