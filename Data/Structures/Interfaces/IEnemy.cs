namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface IEnemy : IHasId
{
    
    public string Name { get; set; }

    public string BattlerName { get; set; }

    public int BattlerHue { get; set; }
}
