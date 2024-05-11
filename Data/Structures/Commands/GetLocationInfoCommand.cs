using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class GetLocationInfoCommand : NamedCommand
    {
        public enum DesignationType
        {
            Direct = 0,
            ByVariables = 1,
        }
        
        private static Dictionary<int, Tuple<string, string>> InfoTypeIdsMap = new()
        {
            [0] = new("Terrain Tag", "Terrain Tag"),
            [1] = new("Event ID", "Event ID"),
            [2] = new("Tile ID (Layer 1)", "Tile ID (Layer 1)"),
            [3] = new("Tile ID (Layer 2)", "Tile ID (Layer 2)"),
            [4] = new("Tile ID (Layer 3)", "Tile ID (Layer 3)"),
            [5] = new("Tile ID (Layer 4)", "Region ID"),
            [6] = new("Region ID", ""),
        };

        public int GetVariable()
        {
            return GetParameter<int>(0);
        }

        public int GetInfoType()
        {
            return GetParameter<int>(1);
        }

        public DesignationType GetDesignationType()
        {
            return (DesignationType)GetParameter<int>(2);
        }

        public int GetParam1()
        {
            return GetParameter<int>(3);
        }

        public int GetParam2()
        {
            return GetParameter<int>(4);
        }

        public override int Code => CommandCodes.GetLocationInfo;
        public override string GetCommandName(StorageVersion version) => "Get Location Info";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            string str;
            if (context.Version == StorageVersion.MV)
                str = context.NameFormatter.FormatVariable(GetVariable(), new() { BracketType = BracketType.None });
            else
                str = context.NameFormatter.FormatVariable(GetVariable(), new()
                {
                    SkipName = true,
                    SkipTitle = true,
                });

            if (!InfoTypeIdsMap.TryGetValue(GetInfoType(), out var infoTypeName))
                throw new Exception($"Unknown info type id {GetInfoType()} for get location info command");

            var infoName = context.Version == StorageVersion.MV ? infoTypeName.Item1 : infoTypeName.Item2;
            if (string.IsNullOrEmpty(infoName))
                throw new Exception($"Unknown info type id {GetInfoType()} for get location info command");

            str += $", {infoName}";

            switch (GetDesignationType())
            {
                case DesignationType.Direct:
                    if (context.Version == StorageVersion.MV)
                        str += $", ({GetParam1()},{GetParam2()})";
                    else
                        str += $", ({GetParam1():D3},{GetParam2():D3})";
                    break;
                case DesignationType.ByVariables:
                    string var1, var2;
                    if (context.Version == StorageVersion.MV)
                    {
                        var1 = context.NameFormatter.FormatVariable(GetParam1());
                        var2 = context.NameFormatter.FormatVariable(GetParam2());

                        str += $", ({var1},{var2})";
                    }
                    else
                    {
                        var1 = context.NameFormatter.FormatVariable(GetParam1(), new()
                        {
                            SkipTitle = true,
                            SkipName = true,
                        });
                        var2 = context.NameFormatter.FormatVariable(GetParam2(), new()
                        {
                            SkipTitle = true,
                            SkipName = true,
                        });

                        str += $", Variable {var1}{var2}";
                    }

                    break;
                default:
                    throw new Exception($"Unknown get location info designation type '{GetDesignationType()}'");
            }

            return str;
        }
    }
}

