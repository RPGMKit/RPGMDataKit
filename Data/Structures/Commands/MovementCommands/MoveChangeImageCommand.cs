using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveChangeImageCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.ChangeImage;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Image";
        else
            return "Graphic";
    }
    
    public string Image
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    public int Offset
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        if (string.IsNullOrWhiteSpace(Image))
            return context.Version == StorageVersion.MV ? "None" : "(None)";

        if (context.Version == StorageVersion.MV)
            return $"{Image}({Offset})";
        else
            return $"'{Image}', {Offset}";
    }
}