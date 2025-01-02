namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IPageGraphic
{
    public int TileId { get; set; }
            
    public string CharacterName { get; set; }
            
    public int CharacterIndex { get; set; }
            
    public int Direction { get; set; }
            
    public int Pattern { get; set; }
}
