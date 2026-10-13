using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ShakeScreenCommand : NamedCommand
{
    public override int Code => CommandCodes.ShakeScreen;
    public override string GetCommandName(StorageVersion version) => "Shake Screen";

    public int Power
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public int Speed
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