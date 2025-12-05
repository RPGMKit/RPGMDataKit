namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IItem
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int IconIndex { get; set; }
}
