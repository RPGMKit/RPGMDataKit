using RPGMDataKit.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::BaseItem")]
public class BaseItem : RubySerializationEntity, INewEntity
{
    [RubyProperty("@id")]
    public int Id { get; set; }

    [RubyProperty("@name")]
    public string Name { get; set; }

    [RubyProperty("@icon_index")]
    public int IconIndex { get; set; }

    [RubyProperty("@description")]
    public string Description { get; set; }

    [RubyProperty("@features")]
    public List<Feature> Features { get; set; }

    [RubyProperty("@note")]
    public string Note { get; set; }

    /// <summary>The values of <c>RPG::BaseItem.new</c> of VX Ace.</summary>
    public virtual void InitializeNew()
    {
        Name = "";
        IconIndex = 0;
        Description = "";
        Features = new();
        Note = "";
    }
}
