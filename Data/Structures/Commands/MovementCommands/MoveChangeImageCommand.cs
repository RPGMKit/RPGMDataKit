using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

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

    // a new command has no image in the editors (an empty name, not none)
    protected override void OnBinding(StorageVersion version)
    {
        if (!IsParameterSet(0))
            Set(0, "");
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