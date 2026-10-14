namespace RPGMDataKit.Data.Structures.Interfaces;

/// <summary>
/// An entity with features (VX Ace) or traits (MV): actors, classes, weapons, armors, enemies, states in both makers.
/// The list is for reading; to change it, use the list of the version (<c>Features</c> or <c>Traits</c>).
/// </summary>
public interface IHasFeatures
{
    public IReadOnlyList<IFeature> Features { get; }
}
