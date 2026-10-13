using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;
using RubyMarshalCS.Serialization.Attributes;
using RubyMarshalCS.Serialization.Enums;

namespace RPGMDataKit.Data.VXAce.Classes;

[RubyObject("RPG::MapInfo")]
public class MapInfo : RubySerializationEntity, IMapInfo, INewEntity
{
    /// <summary>The values of <c>RPG::MapInfo.new</c> of VX Ace: no name, in the root of the tree, collapsed.</summary>
    public void InitializeNew()
    {
        Expanded = false;
        Name = "";
        Order = 0;
        Parent = 0;
        ScrollX = 0;
        ScrollY = 0;
    }

    /// <summary>The number of the map: it is the key of the list in the file, the entry itself has no such field (not saved).</summary>
    public int Id { get; set; }

    [RubyProperty("@expanded")]
    public bool Expanded { get; set; }

    [RubyProperty("@name")]
    public string Name { get; set; }

    [RubyProperty("@order")]
    public int Order { get; set; }

    [RubyProperty("@parent_id")]
    public int Parent { get; set; }

    [RubyProperty("@scroll_x")]
    public int ScrollX { get; set; }

    [RubyProperty("@scroll_y")]
    public int ScrollY { get; set; }
    
    [RubyProperty("@features", CandidateFlags.In | CandidateFlags.Dynamic)]
    public object _1 { get; set; }
}