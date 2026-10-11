using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class FlashScreenCommand : NamedCommand
{
    public override int Code => CommandCodes.FlashScreen;
    public override string GetCommandName(StorageVersion version) => "Flash Screen";

    public IColor Color
    {
        get => GetEntity<IColor>(0);
        set => Set(0, IsDetached ? value : Colors.For(value, Map));
    }

    public int Duration
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public bool WaitForCompletion
    {
        get => Get<bool>(2);
        set => Set(2, value);
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