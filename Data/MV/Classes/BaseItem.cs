using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

/// <summary>
/// What every entity of the database of MV has: the number, the name and the note.
/// MV has no such class (every entity is a plain object of the JSON file); it exists to match <c>RPG::BaseItem</c> of VX Ace.
/// </summary>
public abstract class BaseItem : JsonEntity, IBaseItem, INewEntity
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("note")]
    public string Note { get; set; }

    public virtual void InitializeNew()
    {
        Name = "";
        Note = "";
    }
}
