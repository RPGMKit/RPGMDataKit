namespace RPGMDataKit.Data.Structures.Interfaces;

public interface IMember
{
    public int EnemyId { get; set; }
    
    public int X { get; set; }

    public int Y { get; set; }

    public bool Hidden { get; set; }
}
