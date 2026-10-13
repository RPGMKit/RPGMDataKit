using Newtonsoft.Json;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.MV.Classes;

public class UsableItem : JsonEntity
{
    public class Effect : JsonEntity, IEffect, INewEntity
    {
        /// <summary>The values of a new effect, as in VX Ace (not checked in the editor of MV yet): "Recover HP" with 0 and 0.</summary>
        public void InitializeNew()
        {
            Code = 11;
            DataId = 1;
            Value1 = 0.0;
            Value2 = 0.0;
        }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("dataId")]
        public int DataId { get; set; }

        [JsonProperty("value1")]
        public object Value1 { get; set; }

        [JsonProperty("value2")]
        public object Value2 { get; set; }
    }

    public class ItemDamage : JsonEntity, IItemDamage, INewEntity
    {
        /// <summary>The values that the editor of MV gives to a new damage: no damage, the formula 0, the variance of 20%.</summary>
        public void InitializeNew()
        {
            Type = 0;
            ElementId = 0;
            Formula = "0";
            Variance = 20;
            Critical = false;
        }

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