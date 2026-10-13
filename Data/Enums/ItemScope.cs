namespace RPGMDataKit.Data.Enums;

/// <summary>Who an item or a skill is used on (<c>Scope</c> of a usable item). The same numbers in VX Ace and MV.</summary>
public static class ItemScope
{
    public const int None = 0;
    public const int OneEnemy = 1;
    public const int AllEnemies = 2;
    public const int OneRandomEnemy = 3;
    public const int TwoRandomEnemies = 4;
    public const int ThreeRandomEnemies = 5;
    public const int FourRandomEnemies = 6;
    public const int OneAlly = 7;
    public const int AllAllies = 8;
    public const int OneDeadAlly = 9;
    public const int AllDeadAllies = 10;
    public const int User = 11;
}
