namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IItem : IHasId
{
    
    public string Name { get; set; }
    
    public int IconIndex { get; set; }
}
