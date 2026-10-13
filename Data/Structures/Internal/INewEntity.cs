namespace RPGMDataKit.Data.Structures.Internal;

/// <summary>
/// An entity that knows the values that the editor of its maker gives to a new one (an actor of VX Ace made by <c>RPG::Actor.new</c>,
/// an actor that the editor of MV adds to the database). <see cref="Storages.TypeMap.New{T}"/> calls it right after the constructor.
/// A loaded entity is never touched by it: the constructor, which the serializers use, leaves the fields as they are (null for the ones
/// that are not in the file), so the files keep their format.
/// </summary>
public interface INewEntity
{
    /// <summary>Sets the values of a new entity: empty text, empty lists, the numbers that the editor puts into a new entry.</summary>
    void InitializeNew();
}
