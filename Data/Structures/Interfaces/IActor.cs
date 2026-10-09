namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IActor : IHasId
{
    
    public string Name { get; set; }
    
    public int ClassId { get; set; }
    
    public string NickName { get; set; }
}
