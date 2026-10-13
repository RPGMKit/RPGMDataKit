using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ErasePictureCommand : NamedCommand
{
    public override int Code => CommandCodes.ErasePicture;
    public override string GetCommandName(StorageVersion version) => "Erase Picture";

    public int Layer
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.Version == StorageVersion.MV ? $"#{Layer}" : Layer.ToString();
    }
}