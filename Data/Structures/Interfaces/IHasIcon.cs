namespace RPGMDataKit.Data.Structures.Interfaces;

/// <summary>An entity with an icon (the number of the icon in <c>IconSet</c>): skills, items, weapons, armors, states in both makers.</summary>
/// <remarks>VX Ace keeps the icon in actors, classes and enemies too, but the editor does not use it; MV has no such field there.</remarks>
public interface IHasIcon
{
    public int IconIndex { get; set; }
}
