using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class RotatePictureCommand : NamedCommand
{
    public override int Code => CommandCodes.RotatePicture;
    public override string GetCommandName(StorageVersion version) => "Rotate Picture";

    public int Layer
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

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