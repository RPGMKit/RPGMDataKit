using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class RotatePictureCommand : NamedCommand
{
    public override int Code => CommandCodes.RotatePicture;
    public override string GetCommandName(StorageVersion version) => "Rotate Picture";

    [EditorDefault(1)]
    public int Layer
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    [EditorDefault(5, StorageVersion.VXAce)]   // the editor of MV opens with 0
    public int Speed
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        if (context.Version == StorageVersion.MV)
            return $"#{Layer}, {Speed}";

        if (Speed >= 0)
            return $"{Layer}, +{Speed}";

        return $"{Layer}, {Speed}";
    }
}