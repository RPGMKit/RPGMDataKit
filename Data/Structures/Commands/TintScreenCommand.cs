using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;


public class TintScreenCommand : NamedCommand
{
    public override int Code => CommandCodes.TintScreen;
    public override string GetCommandName(StorageVersion version) => "Tint Screen";

    [EditorDefaultTone(0, 0, 0, 0)]
    public ITone Tone
    {
        get => GetEntity<ITone>(0);
        set => Set(0, value);
    }

    [EditorDefault(60)]
    public int Duration
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    [EditorDefault(true)]
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

        var str = string.Format(fmt, Tone.Red, Tone.Green, Tone.Blue, Tone.Gray, Duration);
        if (WaitForCompletion)
            str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

        return str;
    }
}