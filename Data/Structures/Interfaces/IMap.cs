namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IMap
{
    public int TilesetId { get; set; }

    public string DisplayName { get; set; }

    public int Height { get; set; }

    public int Width { get; set; }

    // Width x Height x layers: ground 1 => ground 2? => bottom object => top object => shadow flags => region id
    public Table Data { get; }

    public IDictionary<int, IEvent?> Events { get; }
    
    public string ParallaxName { get; set; }
    
    public bool ParallaxLoopX { get; set; }
    
    public bool ParallaxLoopY { get; set; }
    
    public int ParallaxSX { get; set; }
    
    public int ParallaxSY { get; set; }
    
    public bool SpecifyBattleback { get; set; }
    
    public string Battleback1Name { get; set; }
    
    public string Battleback2Name { get; set; }
    
    public string Note { get; set; }
}