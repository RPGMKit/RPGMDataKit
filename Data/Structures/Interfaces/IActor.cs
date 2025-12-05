namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IActor
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int ClassId { get; set; }
    
    public string NickName { get; set; }
}
