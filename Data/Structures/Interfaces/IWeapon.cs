namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IWeapon : IHasId
{
    
    public string Name { get; set; }
    
    public int IconIndex { get; set; }
}
