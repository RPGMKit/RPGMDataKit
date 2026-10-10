using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class GetLocationInfoCommand : NamedCommand
{
    public override int Code => CommandCodes.GetLocationInfo;
    public override string GetCommandName(StorageVersion version) => "Get Location Info";
        
    public enum DesignationType
    {
        Direct = 0,
        ByVariables = 1,
    }

    public enum InfoType
    {
        TerrainTag = 0,
        EventId = 1,
        TileIdLayer1 = 2,
        TileIdLayer2 = 3,
        TileIdLayer3 = 4,
        TileIdLayer4 = 5,
        RegionI = 6,
    }

    private static readonly Dictionary<InfoType, Tuple<string, string>> InfoTypeIdsMap = new()
    {
        [InfoType.TerrainTag] = new("Terrain Tag", "Terrain Tag"),
        [InfoType.EventId] = new("Event ID", "Event ID"),
        [InfoType.TileIdLayer1] = new("Tile ID (Layer 1)", "Tile ID (Layer 1)"),
        [InfoType.TileIdLayer2] = new("Tile ID (Layer 2)", "Tile ID (Layer 2)"),
        [InfoType.TileIdLayer3] = new("Tile ID (Layer 3)", "Tile ID (Layer 3)"),
        [InfoType.TileIdLayer4] = new("Tile ID (Layer 4)", "Region ID"),
        [InfoType.RegionI] = new("Region ID", ""),
    };
        
    public int VariableId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public InfoType Type
    {
        get => Get<InfoType>(1);
        set => Set(1, value);
    }

    public DesignationType Designation
    {
        get => Get<DesignationType>(2);
        set => Set(2, value);
    }

    public int Param1
    {
        get => Get<int>(3);
        set => Set(3, value);
    }

    public int Param2
    {
        get => Get<int>(4);
        set => Set(4, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        string str;
        if (context.Version == StorageVersion.MV)
            str = context.NameFormatter.FormatVariable(VariableId, new() { BracketType = BracketType.None });
        else
            str = context.NameFormatter.FormatVariable(VariableId, new()
            {
                SkipName = true,
                SkipTitle = true,
            });

        if (!InfoTypeIdsMap.TryGetValue(Type, out var infoTypeName))
            throw new Exception($"Unknown info type id {Type} for get location info command");

        var infoName = context.Version == StorageVersion.MV ? infoTypeName.Item1 : infoTypeName.Item2;
        if (string.IsNullOrEmpty(infoName))
            throw new Exception($"Unknown info type id {Type} for get location info command");

        str += $", {infoName}";

        switch (Designation)
        {
            case DesignationType.Direct:
                if (context.Version == StorageVersion.MV)
                    str += $", ({Param1},{Param2})";
                else
                    str += $", ({Param1:D3},{Param2:D3})";
                break;
            case DesignationType.ByVariables:
                string var1, var2;
                if (context.Version == StorageVersion.MV)
                {
                    var1 = context.NameFormatter.FormatVariable(Param1);
                    var2 = context.NameFormatter.FormatVariable(Param2);

                    str += $", ({var1},{var2})";
                }
                else
                {
                    var1 = context.NameFormatter.FormatVariable(Param1, new()
                    {
                        SkipTitle = true,
                        SkipName = true,
                    });
                    var2 = context.NameFormatter.FormatVariable(Param2, new()
                    {
                        SkipTitle = true,
                        SkipName = true,
                    });

                    str += $", Variable {var1}{var2}";
                }

                break;
            default:
                throw new Exception($"Unknown get location info designation type '{Designation}'");
        }

        return str;
    }
}