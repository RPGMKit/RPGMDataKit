using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.MV.Classes;

public class UsableItem : JsonEntity
{
    public class Effect : JsonEntity, IEffect
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("dataId")]
        public int DataId { get; set; }

        [JsonProperty("value1")]
        public object Value1 { get; set; }

        [JsonProperty("value2")]
        public object Value2 { get; set; }
    }

    public class ItemDamage : JsonEntity, IItemDamage
    {
        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("elementId")]
        public int ElementId { get; set; }

        [JsonProperty("formula")]
        public string Formula { get; set; }

        [JsonProperty("variance")]
        public int Variance { get; set; }

        [JsonProperty("critical")]
        public bool Critical { get; set; }
    }
}