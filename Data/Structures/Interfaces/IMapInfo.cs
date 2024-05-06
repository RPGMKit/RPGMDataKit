namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IMapInfo
{
    public bool Expanded { get; set; }

    public string Name { get; set; }

    public int Order { get; set; }
    
    public int Parent { get; set; }

    public int ScrollX { get; set; }

    public int ScrollY { get; set; }
}