using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeTilesetCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeTileset;
    public override string GetCommandName(StorageVersion version) => "Change Tileset";

    public int TileSetId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatTileset(TileSetId);
    }
}
