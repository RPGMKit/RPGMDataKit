using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IMap : IDumpable
{
    public int TilesetId { get; set; }

    public string DisplayName { get; set; }

    public int Height { get; set; }

    public int Width { get; set; }

    // ground 1 => ground 2? => bottom object => top object => shadow flags => region id
    public LookupList<int> Data { get; set; }

    public IEnumerable<IEvent> Events { get; }
}