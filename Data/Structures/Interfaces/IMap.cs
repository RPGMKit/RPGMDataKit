using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.Structures.Internal.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IMap
{
    public int TilesetId { get; set; }

    public string DisplayName { get; set; }

    public int Height { get; set; }

    public int Width { get; set; }

    // ground 1 => ground 2? => bottom object => top object => shadow flags => region id
    public IDataLookup<int> Data { get; }

    public IByIdGetter<IEvent> Events { get; }
}