using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeTilesetCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeTileset;
    public override string GetCommandName(StorageVersion version) => "Change Tileset";

    public int TileSetId { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        TileSetId = parameterProvider.GetParameter<int>(0);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, TileSetId);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatTileset(TileSetId);
    }
}
