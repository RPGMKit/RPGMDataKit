using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

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

    [EditorDefault(TypeNone)]
    public string EffectType
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    [EditorDefault(9, StorageVersion.VXAce)]
    [EditorDefault(5, StorageVersion.MV)]
    public int Power
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    [EditorDefault(60, StorageVersion.MV)]   // VX Ace opens with 0
    public int Duration
    {
        get => Get<int>(2);
        set => Set(2, value);
    }

    [EditorDefault(true)]
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