namespace RPGMakerLibCoreV2.Diff;

public class Patch
{
    public class CommonEvenData
    {
        public int CommonEventId;
    }

    public class MapData
    {
        public int MapId;
    }
    
    public class MapInfoData
    {
        public int MapId;
    }

    public class MapEventData
    {
        public int MapId;
        public int EventId;
    }
    
    public class MapEventPageData
    {
        public int MapId;
        public int EventId;
        public int PageId;
    }

    public class SwitchData
    {
        public int SwitchId;
    }

    public class VariableData
    {
        public int VariableId;
    }

    public DiffType Type;
    public SwitchData Switch;
    public VariableData Variable;
    public CommonEvenData CommonEvent;
    public MapData Map;
    public MapInfoData MapInfo;
    public MapEventData MapEvent;
    public MapEventPageData MapEventPage;
    public List<DiffMatchPatch.Patch> Patches;
}
