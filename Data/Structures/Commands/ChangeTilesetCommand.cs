using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeTilesetCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeTileset;
    public override string GetCommandName(StorageVersion version) => "Change Tileset";

    public int GetTilesetId()
    {
        return GetParameter<int>(0);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatTileset(GetTilesetId());
    }
}