using RPGMakerLibCoreV2.Data.Enums;
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

    public string EffectType
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    public int Power
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public int Duration
    {
        get => Get<int>(2);
        set => Set(2, value);
    }

    public bool WaitForCompletion
    {
        get => Get<bool>(3);
        set => Set(3, value);
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