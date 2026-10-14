namespace RPGMDataKit.Data.Structures.Interfaces;

/// <summary>
/// What every entity of the database has in both makers: the number, the name and the note.
/// In VX Ace all of them (actors, classes, skills, items, weapons, armors, enemies, states) are made from <c>RPG::BaseItem</c>;
/// in MV they have no common class, but the same three fields.
/// </summary>
public interface IBaseItem : IHasId
{
    public string Name { get; set; }

    public string Note { get; set; }
}
