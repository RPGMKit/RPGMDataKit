namespace RPGMDataKit.Data.Structures.Interfaces;

public interface IMap
{
    public int TilesetId { get; set; }

    public string DisplayName { get; set; }

    public int Height { get; set; }

    public int Width { get; set; }

    /// <summary>
    /// The tiles: a table of Width x Height x layers, the cell is <c>Data[x, y, layer]</c>. The layers depend on the maker:
    /// VX Ace has 4 (three layers of tiles; the 4th holds the shadows in its low four bits and the region from the 9th bit),
    /// MV has 6 (four layers of tiles, the shadows, the regions). The helpers in <c>MapTileExtensions</c> (<c>GetTile</c>, <c>GetShadow</c>, <c>GetRegion</c>...) hide the difference.
    /// </summary>
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