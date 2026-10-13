namespace RPGMDataKit.Data.Structures.Interfaces;

public interface ITroopPageCondition
{
    public bool TurnEnding { get; set; }

    public bool TurnValid { get; set; }

    public bool EnemyValid { get; set; }

    public bool ActorValid { get; set; }

    public bool SwitchValid { get; set; }

    public int TurnA { get; set; }

    public int TurnB { get; set; }

    public int EnemyIndex { get; set; }

    public int EnemyHp { get; set; }

    public int ActorId { get; set; }

    public int ActorHp { get; set; }

    public int SwitchId { get; set; }
}
