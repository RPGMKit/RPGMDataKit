using Newtonsoft.Json;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;

namespace RPGMDataKit.Data.MV.Classes;

public class Item : UsableItem, IItem
{
    [JsonProperty("itypeId")]
    public int ITypeId { get; set; }

    [JsonProperty("price")]
    public int Price { get; set; }

    [JsonProperty("consumable")]
    public bool Consumable { get; set; }

    /// <summary>The values that the editor of MV gives to a new item: usable anywhere on one friend, a normal item that is used up.</summary>
    public override void InitializeNew()
    {
        base.InitializeNew();
        Scope = ItemScope.OneAlly;
        ITypeId = 1;
        Price = 0;
        Consumable = true;
    }
}
