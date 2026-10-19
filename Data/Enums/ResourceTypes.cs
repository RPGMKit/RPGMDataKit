namespace RPGMDataKit.Data.Enums;

/// <summary>
/// The names of the built-in kinds of resource (<see cref="ResourceType"/>). A kind is known to a storage by its name, which is not case sensitive; the enum is the way to name a built-in one
/// without a string. A kind of your own has no member, only a name (<c>Storage.RegisterResource</c>).
/// </summary>
public static class ResourceTypes
{
    /// <summary>The name of each built-in kind. The names are different from one another when the case is ignored.</summary>
    public static IReadOnlyDictionary<ResourceType, string> Names { get; } = new Dictionary<ResourceType, string>
    {
        [ResourceType.Animation] = "Animation",
        [ResourceType.Battleback1] = "Battleback1",
        [ResourceType.Battleback2] = "Battleback2",
        [ResourceType.Battler] = "Battler",
        [ResourceType.Character] = "Character",
        [ResourceType.Face] = "Face",
        [ResourceType.Parallax] = "Parallax",
        [ResourceType.Picture] = "Picture",
        [ResourceType.SideViewActor] = "SideViewActor",
        [ResourceType.SideViewEnemy] = "SideViewEnemy",
        [ResourceType.System] = "System",
        [ResourceType.Tileset] = "Tileset",
        [ResourceType.Title1] = "Title1",
        [ResourceType.Title2] = "Title2",
        [ResourceType.Bgm] = "Bgm",
        [ResourceType.Bgs] = "Bgs",
        [ResourceType.Me] = "Me",
        [ResourceType.Se] = "Se",
        [ResourceType.Movie] = "Movie",
    };

    private static readonly Dictionary<string, ResourceType> ByName = Names.ToDictionary(p => p.Value, p => p.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>The name of the built-in kind.</summary>
    public static string NameOf(this ResourceType type) => Names[type];

    /// <summary>The built-in kind of a name, the case not mattering; false for a name that is not one of the built-in kinds.</summary>
    public static bool TryParse(string? name, out ResourceType type)
    {
        type = default;

        return name != null && ByName.TryGetValue(name.Trim(), out type);
    }
}
