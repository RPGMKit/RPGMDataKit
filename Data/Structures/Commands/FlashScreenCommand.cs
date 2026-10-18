using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class FlashScreenCommand : NamedCommand
{
    public override int Code => CommandCodes.FlashScreen;
    public override string GetCommandName(StorageVersion version) => "Flash Screen";

    [EditorDefaultColor(255, 255, 255, 255)]
    public IColor Color
    {
        get => GetEntity<IColor>(0);
        set => Set(0, value);
    }

    [EditorDefault(60, StorageVersion.VXAce)]
    public int Duration
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    [EditorDefault(true, StorageVersion.VXAce)]
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