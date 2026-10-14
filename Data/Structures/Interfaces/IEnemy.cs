namespace RPGMDataKit.Data.Structures.Interfaces;

public interface IEnemy : IBaseItem, IHasFeatures
{
    public string BattlerName { get; set; }

    public int BattlerHue { get; set; }
}
