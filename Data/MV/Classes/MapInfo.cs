using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class MapInfo : JsonEntity, IMapInfo, INewEntity
{
    /// <summary>The values that the editor of MV gives to a new map in the list: no name, in the root of the tree, collapsed.</summary>
    public void InitializeNew()
    {
        Expanded = false;
        Name = "";
        Order = 0;
        Parent = 0;
        ScrollX = 0;
        ScrollY = 0;
    }

    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }

    [JsonProperty(PropertyName = "expanded")]
    public bool Expanded { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "order")]
    public int Order { get; set; }
    
    [JsonProperty(PropertyName = "parentId")]
    public int Parent { get; set; }

    [JsonProperty(PropertyName = "scrollX")]
    public float ScrollXFloat { get; set; }

    [JsonProperty(PropertyName = "scrollY")]
    public float ScrollYFloat { get; set; }
    
    public int ScrollX
    {
        get => (int)ScrollXFloat;
        set => ScrollXFloat = value;
    }

    public int ScrollY
    {
        get => (int)ScrollYFloat;
        set => ScrollYFloat = value;
    }
}