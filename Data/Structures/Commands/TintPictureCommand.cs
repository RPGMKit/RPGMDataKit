using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;


public class TintPictureCommand : NamedCommand
{
    public override int Code => CommandCodes.TintPicture;
    public override string GetCommandName(StorageVersion version) => "Tint Picture";

    public int Layer
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public ITone Tone
    {
        get => GetEntity<ITone>(1);
        set => Set(1, IsDetached ? value : Tones.For(value, Map));
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
        var fmt = context.Version == StorageVersion.MV
            ? "#{0}, ({1},{2},{3},{4}), {5} frame(s)"
            : "{0}, ({1},{2},{3},{4}), @{5}";

        var str = string.Format(fmt, Layer, Tone.Red, Tone.Green, Tone.Blue, Tone.Gray, Duration);
        if (WaitForCompletion)
            str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

        return str;
    }
}