namespace RPGMDataKit.Storages.Abstraction;

/// <summary>An entity whose own id differs from its place in a list (the index) or in a dictionary (the key); see <see cref="Storage.IdMismatched"/>.</summary>
/// <param name="File">The data file, e.g. <c>Quests.json</c>.</param>
/// <param name="Key">The place of the entity: the id that the engines look it up by.</param>
/// <param name="EntityId">The id written in the entity.</param>
/// <param name="Entity">The entity.</param>
public sealed record IdMismatch(string File, int Key, int EntityId, object Entity);
