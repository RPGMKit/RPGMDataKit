namespace RPGMDataKit.Data.Structures.Interfaces;

/// <summary>
/// A map in the list of maps. <see cref="IHasId.Id"/> is the number of the map: in MV it is a field of the entry, in VX Ace only the key of the
/// list, so there it is not saved with the entry.
/// </summary>
public interface IMapInfo : IHasId
{
    public bool Expanded { get; set; }

    public string Name { get; set; }

    public int Order { get; set; }
    
    public int Parent { get; set; }

    public int ScrollX { get; set; }

    public int ScrollY { get; set; }
}