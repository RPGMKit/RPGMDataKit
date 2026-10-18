using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;


public class TintPictureCommand : NamedCommand
{
    public override int Code => CommandCodes.TintPicture;
    public override string GetCommandName(StorageVersion version) => "Tint Picture";

    [EditorDefault(1, StorageVersion.VXAce)]
    public int Layer
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    [EditorDefaultTone(0, 0, 0, 0)]
    public ITone Tone
    {
        get => GetEntity<ITone>(1);
        set => Set(1, value);
    }

    [EditorDefault(60, StorageVersion.VXAce)]
    public int Duration
    {
        get => Get<int>(2);
        set => Set(2, value);
    }

    [EditorDefault(true, StorageVersion.VXAce)]
    public bool WaitForCompletion
    {
        get => Get<bool>(3);
        set => Set(3, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var fmt = context.Version == StorageVersion.MV
            ? "#{0}, ({1},{2},{3},{4}), {5} frame(s)"
            : "{0}, ({1},{2},{3},{4}), @{5}";

        var str = string.Format(fmt, Layer, Tone.Red, Tone.Green, Tone.Blue, Tone.Gray, Duration);
        if (WaitForCompletion)
            str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

        return str;
    }
}